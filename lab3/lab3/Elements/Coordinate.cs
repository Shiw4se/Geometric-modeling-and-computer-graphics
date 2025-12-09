using System.Collections.Generic;

namespace lab3;

public class Coordinate
{
    // O - центр, P_Right/Up/Left/Down - кінці головних осей
    public static Point O, P_Right, P_Up, P_Left, P_Down;
    
    public double PixelInSm;
    public static List<Point> Points; // Список важливих точок для трансформацій
    
    public static List<Point> XLines; // Горизонтальні лінії сітки
    public static List<Point> YLines; // Вертикальні лінії сітки

    public Coordinate(double px)
    {
        Points = new List<Point>();
        PixelInSm = px;
        
        // 1. ЦЕНТР ЕКРАНУ
        // Для троянди (полярні координати) ідеально мати центр посередині.
        // Ширина Canvas ~1200, Висота ~900 -> Центр (600, 450)
        O = new Point(600, 450); 

        // 2. ДІАПАЗОН СІТКИ (на скільки малювати в боки)
        double range = 800; 

        // 3. ГОЛОВНІ ОСІ (Хрест координат)
        P_Right = new Point(O.X + range, O.Y); // X+
        P_Left  = new Point(O.X - range, O.Y); // X-
        P_Up    = new Point(O.X, O.Y - range); // Y+ (вгору)
        P_Down  = new Point(O.X, O.Y + range); // Y- (вниз)

        // Додаємо в список, щоб вони теж рухалися при трансформаціях
        Points.Add(O);
        Points.Add(P_Right);
        Points.Add(P_Left);
        Points.Add(P_Up);
        Points.Add(P_Down);
        
        FillLines(range);
    }

    private void FillLines(double range)
    {
        XLines = new List<Point>();
        YLines = new List<Point>();

        // Крок сітки (наприклад, кожні 20 пікселів або 10 одиниць * масштаб)
        double step = PixelInSm * 20; 

        // ВЕРТИКАЛЬНІ ЛІНІЇ СІТКИ (|||||)
        // Йдемо від лівого краю (-range) до правого (+range)
        for (double i = -range; i <= range; i += step)
        {
            var pTop = new Point(O.X + i, O.Y - range); // Верх
            var pBot = new Point(O.X + i, O.Y + range); // Низ
            
            YLines.Add(pTop);
            YLines.Add(pBot);
        }

        // ГОРИЗОНТАЛЬНІ ЛІНІЇ СІТКИ (=====)
        // Йдемо від низу (-range) до верху (+range)
        for (double i = -range; i <= range; i += step)
        {
            var pLeft = new Point(O.X - range, O.Y - i);  // Ліво
            var pRight = new Point(O.X + range, O.Y - i); // Право
            
            XLines.Add(pLeft);
            XLines.Add(pRight);
        }
    }
}