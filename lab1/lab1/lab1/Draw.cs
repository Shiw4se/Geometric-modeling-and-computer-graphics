using System.Runtime.Intrinsics.X86;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace lab1
{
    public class Draw
    {
        private readonly  Canvas _canvas;
    public double _pixelsInSm;
    public View View;

    public Draw(Canvas canvas, double pixelsInSm,View view )
    {
           _canvas = canvas;
           _pixelsInSm=pixelsInSm;
           View = view;
    }

    public void Drawer()
    {
        _canvas.Children.Clear();
        DrawSystem();

        // ... (попередні лінії) ...
        DrawLine(View.A, View.B);
        DrawLine(View.B, View.C);
        DrawLine(View.C, View.D);
        DrawLine(View.D, View.A);

        DrawLine(View.E, View.F);
        DrawLine(View.G, View.H);

        DrawLine(View.I, View.J);
        DrawLine(View.K, View.L);
        
        DrawLine(View.R, View.S); 
        DrawLine(View.M, View.T);

        // Коло
        CalculateCircle(View.rSmall, 0, 360, View.N, View.P);
        DrawCircle(View.Circle);
    }

    public void TransformDraw()
    {
        _canvas.Children.Clear();
        DrawSystem();

        // Зовнішній контур
        DrawLine(View.A, View.B);
        DrawLine(View.B, View.C);
        DrawLine(View.C, View.D);
        DrawLine(View.D, View.A);

        // Внутрішні лінії
        DrawLine(View.E, View.F);
        DrawLine(View.G, View.H);
    
        // Горизонтальні штрихи (якщо треба, наприклад на висоті 12)
        // Можна домалювати тут динамічно, якщо не хочете створювати точки в моделі
        DrawLine(View.I, View.J); // Нижня перемичка
        DrawLine(View.K, View.L); // Верхня перемичка
        DrawLine(View.R, View.S); 
        DrawLine(View.M, View.T);
        // Коло
        CalculateCircle(View.rSmall, 0, 360, View.N, View.P);
        DrawCircle(View.Circle);
    }

    public void DrawLine(Point pbeg, Point pend)
    {
        var ln = new System.Windows.Shapes.Line
        {
            Stroke = Brushes.Black,
            StrokeThickness = 2,
            X1 = pbeg.X,
            Y1 = pbeg.Y,
            X2 = pend.X,
            Y2 = pend.Y
        };
        _canvas.Children.Add(ln);
    }

    public void CalculateCircle(double r, double degreeBeg, double degreeEnd, Point beg, Point end)
    {

        r *= _pixelsInSm;
        double xc = (beg.X + end.X) / 2;
        double yc = (beg.Y + end.Y) / 2;

        int c = 0;
        for (double i = degreeBeg; i < degreeEnd; ++i)
        {
            var f = DegToRad(i);
            var v = DegToRad(i + 1);

            var pbeg = new Point
            {
                X = r * Math.Cos(f) + xc,
                Y = r * Math.Sin(f) + yc
            };

            var pend = new Point
            {
                X = r * Math.Cos(v) + xc,
                Y = r * Math.Sin(v) + yc
            };

            // Перевірка, щоб не вийти за межі масиву
            if (c < View.Circle.Length - 1)
            {
                View.Circle[c] = pbeg;
                View.Circle[c + 1] = pend;
                c += 2;
            }
        }
    }

    public void CalculateArc(double r, double degreeBeg, double degreeEnd, Point beg, Point end)
    {
        
        r *=_pixelsInSm;
        
        double xc = (beg.X+end.X)/2, yc = (beg.Y+end.Y)/2;
        var c = 0;
        for(double i = degreeBeg; i < degreeEnd; ++i)
        {
            var f = DegToRad(i);
            var v= DegToRad(i + 1);
            var pbeg = new Point
            {
                X=r*Math.Cos(f)+xc,
                Y=r*Math.Sin(f)+yc
            };

            var pend = new Point
            {
                X=r*Math.Cos(v)+xc,
                Y=r*Math.Sin(v)+yc
            };
            View.Arc[c]=pbeg;
            View.Arc[c+1]=pend;
            c += 2;
            //DrawLine(pend, pbeg);
        }
    }
    
    public void DrawCircle(Point[] arc)
    {
        // 1. Перевірка, чи сам масив існує
        if (arc == null) return;

        for (int i = 0; i < arc.Length - 1; i += 2)
        {
            // 2. ВАЖЛИВО: Перевіряємо, чи існують точки в цій комірці масиву
            // Якщо точок немає (null), значить ми дійшли до кінця намальованого кола
            if (arc[i] != null && arc[i + 1] != null)
            {
                DrawLine(arc[i], arc[i + 1]);
            }
            else
            {
                // Якщо зустріли null, далі малювати немає сенсу — виходимо з циклу
                break;
            }
        }
    }
    
    private static double DegToRad(double deg)
    {
        return deg*Math.PI / 180;
    }

    public double[] CalculateDegree(Point p1, Point p2,double rad)
    {
        
        
        var distance=Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p1.Y - p1.Y, 2))/_pixelsInSm;
        
        var degBeg=Math.Asin(distance/(rad*2))*(180/Math.PI);
        if (p1.Y < p2.Y)
        {
            if (p1.X == p2.X)
                {
                    return [90,270];
                }
            if (p1.X > p2.X)
            {
                return [90+degBeg, 90+degBeg+180];
            }
            return [90-degBeg, 90-degBeg+180];
                    //else return [90-degBeg, 90-degBeg+180];
        }
        
        if (p1.Y > p2.Y)
        {
            if (p1.X == p2.X)
            {
                return [270,360];
            }
            if (p1.X > p2.X)
            {
                return [270-degBeg, 270-degBeg+180];
            }
            return [270+degBeg, 270+degBeg+180];
            
        }

        if (p2.X < p1.X)
        {
            return [90+degBeg,270+degBeg];
        }
        return [90-degBeg+180, 90-degBeg];
    }

    public void DrawSystem()
    {
        // --- 1. ГОЛОВНІ ОСІ КООРДИНАТ (X та Y) ---
        // Малюємо їх першими. 
        // DrawLine робить лінію чорною (а сітка буде сірою).
        
        DrawLine(CoordinateSystem.O, CoordinateSystem.P1); // Вісь X
        DrawLine(CoordinateSystem.O, CoordinateSystem.P2); // Вісь Y

        // --- 2. СІТКА ---
        // Вертикальні лінії сітки
        for (int i = 0; i < CoordinateSystem.YLines.Count; i += 2)
        {
            DrawBackLine(CoordinateSystem.YLines[i], CoordinateSystem.YLines[i+1]);
        }

        // Горизонтальні лінії сітки
        for (int i = 0; i < CoordinateSystem.XLines.Count; i += 2)
        {
            DrawBackLine(CoordinateSystem.XLines[i], CoordinateSystem.XLines[i+1]);
        }
    }

    private void DrawBackLine(Point pbeg, Point pend)
    {
        var ln = new System.Windows.Shapes.Line
        {
            Stroke = Brushes.LightGray, // Дуже світлий колір, щоб не заважав
            StrokeThickness = 1,
            X1 = pbeg.X,
            Y1 = pbeg.Y,
            X2 = pend.X,
            Y2 = pend.Y
        };
        _canvas.Children.Add(ln);
        
    }

    public double ClaculateRad() =>
        Math.Sqrt(Math.Pow(View.N.X - View.P.X, 2) + Math.Pow(View.N.Y - View.P.Y, 2)) /
               _pixelsInSm / 2;
    
    
    }
}