// File: Draw.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace lab3
{
    public class Draw
    {
        private static Canvas _canvas;
        public double _pixelsInSm;
        public bool isAnimated = true;
        private Model _model;

        public bool ShowPoints = true;
        public bool EnableEditing = true;

        // Чому потрібний поріг: щоб "підняти перо" між різними фігурами (сонце/острів/стовбур/листя)
        private const double BreakThreshold = 40.0;

        public Draw(Canvas canvas, double pixelsInSm, Model model)
        {
            _canvas = canvas;
            _pixelsInSm = pixelsInSm;
            _model = model;
        }

        public void DrawSystem()
        {
            DrawLine(Coordinate.P_Left, Coordinate.P_Right);
            DrawLine(Coordinate.P_Down, Coordinate.P_Up);

            for (int i = 0; i < Coordinate.XLines.Count; i += 2)
                if (i + 1 < Coordinate.XLines.Count)
                    DrawBackLine(Coordinate.XLines[i], Coordinate.XLines[i + 1]);

            for (int i = 0; i < Coordinate.YLines.Count; i += 2)
                if (i + 1 < Coordinate.YLines.Count)
                    DrawBackLine(Coordinate.YLines[i], Coordinate.YLines[i + 1]);
        }

        public void DrawModel()
        {
            _canvas.Children.Clear();
            DrawSystem();

            // 1) Каркас: малюємо пунктир лише всередині кожного сегмента (P0-P1-P2)
            if (ShowPoints && _model.Points != null && _model.Points.Count >= 3)
            {
                for (int i = 0; i <= _model.Points.Count - 3; i += 3)
                {
                    var p0 = _model.Points[i];
                    var p1 = _model.Points[i + 1];
                    var p2 = _model.Points[i + 2];

                    DrawDashedLine(p0, p1);
                    DrawDashedLine(p1, p2);
                }

                // Точки
                foreach (var pt in _model.Points)
                    _canvas.Children.Add(CreatePoint(pt));
            }

            // 2) Гладка крива: пропускаємо великі стрибки (розрив пера)
            if (_model.CurvePoints != null && _model.CurvePoints.Count > 1)
                DrawCurve(_model.CurvePoints, 2);
        }

        private Ellipse CreatePoint(Point coord)
        {
            Brush color = coord.isControl ? Brushes.Green : Brushes.Red;

            var point = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = color,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                ToolTip = $"{coord.PointName}: {coord.X:F0}, {coord.Y:F0}",
                Tag = coord
            };

            Canvas.SetLeft(point, coord.X - 5);
            Canvas.SetTop(point, coord.Y - 5);

            if (EnableEditing)
            {
                point.MouseLeftButtonDown += Point_MouseLeftButtonDown;
                point.MouseLeftButtonUp += Point_MouseLeftButtonUp;
                point.MouseMove += Point_MouseMove;
            }

            return point;
        }

        // ---- Drag & Drop ----
        private bool isDragging = false;
        private Ellipse draggedEllipse = null;

        private void Point_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            draggedEllipse = sender as Ellipse;
            draggedEllipse?.CaptureMouse();
        }

        private void Point_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            if (draggedEllipse != null)
            {
                draggedEllipse.ReleaseMouseCapture();
                draggedEllipse = null;

                _model.CreateCurvePoints();
                DrawModel();
            }
        }


        private void Point_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging || draggedEllipse == null) return;

            var mousePos = e.GetPosition(_canvas);

            Canvas.SetLeft(draggedEllipse, mousePos.X - 5);
            Canvas.SetTop(draggedEllipse, mousePos.Y - 5);

            var pointData = draggedEllipse.Tag as Point;
            if (pointData != null)
            {
                pointData.X = mousePos.X;
                pointData.Y = mousePos.Y;
            }
        }

        // ---- Лінії ----

        public void DrawLine(Point pbeg, Point pend, double thickness = 1)
        {
            var ln = new System.Windows.Shapes.Line
            {
                Stroke = Brushes.Black,
                StrokeThickness = thickness,
                X1 = pbeg.X,
                Y1 = pbeg.Y,
                X2 = pend.X,
                Y2 = pend.Y
            };
            _canvas.Children.Add(ln);
        }

        private void DrawBackLine(Point pbeg, Point pend)
        {
            var ln = new System.Windows.Shapes.Line
            {
                Stroke = Brushes.LightGray,
                StrokeThickness = 1,
                X1 = pbeg.X,
                Y1 = pbeg.Y,
                X2 = pend.X,
                Y2 = pend.Y
            };
            _canvas.Children.Add(ln);
        }

        private void DrawDashedLine(Point pbeg, Point pend)
        {
            var ln = new System.Windows.Shapes.Line
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection { 4, 2 },
                X1 = pbeg.X,
                Y1 = pbeg.Y,
                X2 = pend.X,
                Y2 = pend.Y
            };
            _canvas.Children.Add(ln);
        }

        // Малюємо криву з розривами між далекими точками
        private void DrawCurve(IReadOnlyList<Point> pts, double thickness)
        {
            for (int i = 0; i < pts.Count - 1; i++)
            {
                var a = pts[i];
                var b = pts[i + 1];

                if (ShouldBreak(a, b)) continue; // «підняти перо»

                DrawLine(a, b, thickness);
            }
        }

        private static bool ShouldBreak(Point a, Point b)
        {
            if (double.IsNaN(a.X) || double.IsNaN(a.Y) || double.IsNaN(b.X) || double.IsNaN(b.Y))
                return true;

            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            double dist2 = dx * dx + dy * dy;

            return dist2 > BreakThreshold * BreakThreshold;
        }

        // ---- Анімація (без змін логіки) ----
        private List<Point> _originalPoints;

        public async Task DrawAnimation()
        {
            if (!isAnimated) { isAnimated = true; }

            _originalPoints = new List<Point>();
            foreach (var p in _model.Points)
                _originalPoints.Add(new Point(p.X, p.Y, p.PointName, p.isControl));

            var targetPoints = GenerateTargetShape(_model.Points.Count);

            await AnimateTransition(_model.Points, targetPoints, 20);
            await Task.Delay(500);
            await AnimateTransition(_model.Points, _originalPoints, 20);

            for (int i = 0; i < _model.Points.Count; i++)
            {
                _model.Points[i].X = _originalPoints[i].X;
                _model.Points[i].Y = _originalPoints[i].Y;
            }

            _model.CreateCurvePoints();
            DrawModel();
        }

        private async Task AnimateTransition(List<Point> currentPoints, List<Point> targetPoints, int totalSteps)
        {
            for (int step = 1; step <= totalSteps; step++)
            {
                if (!isAnimated) return;

                for (int i = 0; i < currentPoints.Count; i++)
                {
                    if (i >= targetPoints.Count) break;

                    double dx = targetPoints[i].X - currentPoints[i].X;
                    double dy = targetPoints[i].Y - currentPoints[i].Y;


                    double factor = 0.2; // чому: просте зближення без артефактів
                    currentPoints[i].X += dx * factor;
                    currentPoints[i].Y += dy * factor;
                }

                _model.CreateCurvePoints();
                DrawModel();
                await Task.Delay(30);
            }
        }

        private double Lerp(double start, double end, double t) => start + (end - start) * t;

        private List<Point> GenerateTargetShape(int count)
        {
            var target = new List<Point>();
            var p1 = new Point(300, 500);
            var p2 = new Point(600, 100);
            var p3 = new Point(900, 500);

            for (int i = 0; i < count; i++)
            {
                if (i % 3 == 0) target.Add(new Point(p1.X, p1.Y));
                else if (i % 3 == 1) target.Add(new Point(p2.X, p2.Y));
                else target.Add(new Point(p3.X, p3.Y));
            }
            return target;
        }
    }
}
