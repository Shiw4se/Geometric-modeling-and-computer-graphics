using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace lab3;

public class Draw
{
    private static Canvas _canvas;
    public double _pixelsInSm;
    public bool isAnimated = true;
    private Model _model;
    
    public bool ShowPoints = true;
    public bool EnableEditing = true;

    // Threshold to "lift the pen" (avoid drawing lines between distant points, e.g., separate leaves)
    private const double BreakThreshold = 40.0; 

    public Draw(Canvas canvas, double pixelsInSm, Model model)
    {
        _canvas = canvas;
        _pixelsInSm = pixelsInSm;
        _model = model;
    }
    
    public void DrawSystem()
    {
        // Use the NEW names from CoordinateSystem (P_Right, P_Up, etc.)
        DrawLine(Coordinate.O, Coordinate.P_Right); // X+
        DrawLine(Coordinate.O, Coordinate.P_Up);    // Y+
        DrawLine(Coordinate.O, Coordinate.P_Left);  // X-
        DrawLine(Coordinate.O, Coordinate.P_Down);  // Y-
        
        for (int i = 0; i < Coordinate.XLines.Count; i += 2)
        {
            if (i + 1 < Coordinate.XLines.Count)
                DrawBackLine(Coordinate.XLines[i], Coordinate.XLines[i+1]);
        }
        for (int i = 0; i < Coordinate.YLines.Count; i += 2)
        {
            if (i + 1 < Coordinate.YLines.Count)
                DrawBackLine(Coordinate.YLines[i], Coordinate.YLines[i+1]);
        }
    }

    public void DrawModel()
    {
        _canvas.Children.Clear();
        DrawSystem();

        // 1. Draw Control Polygon (Skeleton)
        if (ShowPoints && _model.Points != null && _model.Points.Count >= 3)
        {
            for (int i = 0; i < _model.Points.Count - 1; i++)
            {
                // Only draw dashed lines between control points of the same segment
                // Segments are triplets: 0-1-2, 3-4-5...
                // So we connect i -> i+1.
                // We avoid connecting end of one segment (2) to start of next (3) if they are separate logic.
                
                // Logic: 0->1, 1->2 (draw). 2->3 (don't draw if it's a jump, but our create points logic often duplicates points).
                // Let's draw all sequential points for visualization purposes.
                
                DrawDashedLine(_model.Points[i], _model.Points[i+1]);
                
                Ellipse ellipse = CreatePoint(_model.Points[i]);
                _canvas.Children.Add(ellipse);

                if (i == _model.Points.Count - 2)
                {
                    Ellipse ellipseLast = CreatePoint(_model.Points[i+1]);
                    _canvas.Children.Add(ellipseLast);
                }
            }
        }
        
        // 2. Draw Smooth Curve
        if (_model.CurvePoints != null && _model.CurvePoints.Count > 1)
        {
            for (int i = 0; i < _model.CurvePoints.Count - 1; i++)
            {
                // Check distance to avoid drawing lines between separate segments if they are far apart
                if (GetDistance(_model.CurvePoints[i], _model.CurvePoints[i+1]) < BreakThreshold)
                {
                    DrawLine(_model.CurvePoints[i], _model.CurvePoints[i+1], 2);
                }
            }
        }
    }

    private double GetDistance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }

    private Ellipse CreatePoint(Point coord)
    {
        Brush color = Brushes.Red;
        if (coord.isControl) color = Brushes.Green;

        Ellipse point = new Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = color,
            Stroke = Brushes.Black,
            StrokeThickness = 1,
            ToolTip = $"{coord.PointName}: {coord.X:F0}, {coord.Y:F0}",
            Tag = coord // Store reference to Point object
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

    // --- Drag & Drop ---
    private bool isDragging = false;
    private Ellipse draggedEllipse = null;

    private void Point_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        isDragging = true;
        draggedEllipse = sender as Ellipse;
        draggedEllipse.CaptureMouse();
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
        if (isDragging && draggedEllipse != null)
        {
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
    }

    // --- Line Drawing Helpers ---

    public void DrawLine(Point pbeg, Point pend, double thickness = 1)
    {
        var ln = new System.Windows.Shapes.Line
        {
            Stroke = Brushes.Black,
            StrokeThickness = thickness,
            X1 = pbeg.X, Y1 = pbeg.Y,
            X2 = pend.X, Y2 = pend.Y
        };
        _canvas.Children.Add(ln);
    }
    
    private void DrawBackLine(Point pbeg, Point pend)
    {
        var ln = new System.Windows.Shapes.Line
        {
            Stroke = Brushes.LightGray,
            StrokeThickness = 1,
            X1 = pbeg.X, Y1 = pbeg.Y,
            X2 = pend.X, Y2 = pend.Y
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
            X1 = pbeg.X, Y1 = pbeg.Y,
            X2 = pend.X, Y2 = pend.Y
        };
        _canvas.Children.Add(ln);
    }

    // --- Animation Logic ---
    private List<Point> _originalPoints;

    public async Task DrawAnimation()
    {
        if (!isAnimated) { isAnimated = true; }

        // Store original points
        _originalPoints = new List<Point>();
        foreach (var p in _model.Points) _originalPoints.Add(new Point(p.X, p.Y, p.PointName, p.isControl));

        // Generate triangle target
        var targetPoints = GenerateTargetShape(_model.Points.Count);

        // 1. Morph to Triangle
        await AnimateTransition(_model.Points, targetPoints, 20); 
        await Task.Delay(500); 
        // 2. Morph back to Palm
        await AnimateTransition(_model.Points, _originalPoints, 20);
        
        // Ensure perfect restoration
        for(int i=0; i<_model.Points.Count; i++) {
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

            // Simple linear interpolation towards target
            for (int i = 0; i < currentPoints.Count; i++)
            {
                if (i < targetPoints.Count)
                {
                    double dx = targetPoints[i].X - currentPoints[i].X;
                    double dy = targetPoints[i].Y - currentPoints[i].Y;
                    
                    double factor = 0.2; // Speed factor
                    
                    currentPoints[i].X += dx * factor;
                    currentPoints[i].Y += dy * factor;
                }
            }

            _model.CreateCurvePoints();
            DrawModel();
            await Task.Delay(30);
        }
    }

    private List<Point> GenerateTargetShape(int count)
    {
        var target = new List<Point>();
        // Triangle vertices
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