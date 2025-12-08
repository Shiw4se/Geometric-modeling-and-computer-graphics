using System.Collections.Generic;

namespace lab1;

public class CoordinateSystem
{
    public static Point Px, Py, O, P1, P2;
    public double PixelInSm;
    public static List<Point> Points;
    public static List<Point> XLines; 
    public static List<Point> YLines; 

    public CoordinateSystem(double px)
    {
        Points = new List<Point>();
        PixelInSm = px;
        
        // Центр координат (зліва знизу)
        O = new Point(50, 750); 

        // --- ЗМІНА ТУТ ---
        // Повертаємо менший розмір сітки, щоб не було артефактів при проекції.
        // 550 пікселів достатньо, щоб покрити більшу частину екрану.
        double range = 600; 

        P1 = new Point(O.X + range, O.Y); 
        P2 = new Point(O.X, O.Y - range); 
        
        Px = new Point(O.X + PixelInSm * 10, O.Y);
        Py = new Point(O.X, O.Y - PixelInSm * 10);

        Points.Add(O);
        Points.Add(Px);
        Points.Add(Py);
        Points.Add(P1);
        Points.Add(P2);
        
        FillLines(range);
    }

    private void FillLines(double range)
    {
        XLines = new List<Point>();
        YLines = new List<Point>();

        double step = 25; 

        // Вертикальні лінії
        for (double i = -50; i <= range; i += step) // Починаємо трохи зліва (-50), щоб закрити кут
        {
            var pTop = new Point(O.X + i, O.Y - range);
            var pBot = new Point(O.X + i, O.Y + 50); // Трохи вниз (+50)
            
            YLines.Add(pTop);
            YLines.Add(pBot);
        }

        // Горизонтальні лінії
        for (double i = -50; i <= range; i += step)
        {
            var pLeft = new Point(O.X - 50, O.Y - i);
            var pRight = new Point(O.X + range, O.Y - i);
            
            XLines.Add(pLeft);
            XLines.Add(pRight);
        }
    }
}