namespace lab1;

public class View
{
    public static Point A, B, C, D, E, F, G, H, N, P, I, J, K, L, M, R, S, T;
    public static List<Point> Points;
    public static double PixelsInSm { get; set; } 
    public static double rBig, rSmall ;
    public static Point[] Circle;
    public static Point[] Arc;
    public static double Ox, Oy;

    public  View(double pix)
    {
        Circle = new Point[725]; // Ініціалізація масиву для кола
        
        // ВАЖЛИВО: Створюємо екземпляри точок, щоб вони не були null
        A = new Point();
        B = new Point();
        C = new Point();
        D = new Point();
        E = new Point();
        F = new Point();
        G = new Point();
        H = new Point();
        N = new Point();
        P = new Point();
        I = new Point();
        J = new Point();
        K = new Point();
        L = new Point();
        M = new Point();
        R = new Point();
        S = new Point();
        T = new Point();

        // Заповнюємо список точок строго в тому ж порядку, що і в Model!
        Points = new List<Point> { A, B, C, D, E, F, G, H, N, P, I, J, K, L, M, R, S, T };
        
        rSmall = 10.5;
    }

    public void Clone()
    {
        // Перевіряємо, щоб не вийти за межі масиву, якщо кількість точок різна
        int count = Math.Min(Points.Count, Model.Points.Count);

        for (int i = 0; i < count; i++)
        {
            // Беремо координати з моделі
            var pNew = Model.Points[i].Clone();
            
            // Присвоюємо їх точкам у View
            Points[i]._x = pNew._x;
            Points[i]._y = pNew._y;
        }

        // Копіюємо радіус
        rSmall = Model.rSmall;
    }
}