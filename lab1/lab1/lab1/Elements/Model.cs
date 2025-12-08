using System.Collections.Generic;
using System.Linq;

namespace lab1;

public class Model
{
    public static Point A, B, C, D, E, F, G, H, N, P, I, J, K, L, M, R, S, T;
    public static List<Line> Lines;
    public static List<Point> Points;
    public static double PixelsInSm { get; set; }
    public static double rSmall;
    public static double Ox, Oy;

    public Model(double pix)
    {
        Ox = 50;  
        Oy = 750; 
        
        PixelsInSm = pix;
        Lines = new List<Line>();

        // 1. Ініціалізація точок
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

        Points = new List<Point> { A, B, C, D, E, F, G, H, N, P, I, J, K, L, M, R, S, T };

        // 2. Створення ліній (ОБОВ'ЯЗКОВО додати їх в список, щоб DrawBegin їх бачив)
        
        // Основні габарити (AB - висота, BC - ширина)
        Lines.Add(new Line("AB", A, B, 100)); 
        Lines.Add(new Line("BC", B, C, 50));
        
        // Решта ліній (щоб програма не падала, якщо ми захочемо їх міняти)
        Lines.Add(new Line("CD", C, D, 100));
        Lines.Add(new Line("DA", D, A, 50));
        
        // Внутрішні лінії (поки дамо їм довжину 0 або приблизну, вона перерахується в DrawBegin)
        Lines.Add(new Line("EF", E, F, 100));
        Lines.Add(new Line("GH", G, H, 100));
        Lines.Add(new Line("IJ", I, J, 27));
        Lines.Add(new Line("KL", K, L, 27));
        Lines.Add(new Line("SideBot", R, S, 11.5));
        Lines.Add(new Line("SideTop", M, T, 11.5));

        rSmall = 10.5;

        // 3. Тепер можна безпечно викликати DrawBegin
        DrawBegin();
    }

    public void DrawBegin()
{
   // --- 1. Зовнішній прямокутник ---
    A._x = Ox; 
    A._y = Oy;

    // AB (Ліва вертикаль - Висота)
    var lineAB = Lines.First(p => p.Name == "AB");
    B._x = A._x;
    B._y = A._y - lineAB.Distance * PixelsInSm;
    lineAB.P1 = A; lineAB.P2 = B;

    // BC (Верхня горизонталь - Ширина)
    var lineBC = Lines.First(p => p.Name == "BC");
    C._x = B._x + lineBC.Distance * PixelsInSm;
    C._y = B._y;
    lineBC.P1 = B; lineBC.P2 = C;

    // CD (Права вертикаль)
    var lineCD = Lines.First(p => p.Name == "CD");
    D._x = C._x;
    D._y = C._y + lineCD.Distance * PixelsInSm; 
    lineCD.P1 = C; lineCD.P2 = D;

    // DA (Нижня горизонталь)
    var lineDA = Lines.First(p => p.Name == "DA");
    lineDA.P1 = D; lineDA.P2 = A;

    // --- 2. Внутрішні вертикальні лінії ---
    // Відступ розраховуємо автоматично, щоб лінія була по центру
    // Ширина деталі мінус ширина внутрішньої частини (приблизно)
    double offset = 11.5 * PixelsInSm; 

    // EF (Ліва внутрішня)
    var lineEF = Lines.First(p => p.Name == "EF");
    E._x = A._x + offset; 
    E._y = A._y;
    
    F._x = E._x; 
    F._y = E._y - lineEF.Distance * PixelsInSm; 
    lineEF.P1 = E; lineEF.P2 = F;

    // GH (Права внутрішня)
    var lineGH = Lines.First(p => p.Name == "GH");
    G._x = D._x - offset; 
    G._y = D._y;
    
    H._x = G._x; 
    H._y = G._y - lineGH.Distance * PixelsInSm; 
    lineGH.P1 = G; lineGH.P2 = H;

    // --- 3. Горизонтальні перемички ---
    
    // IJ (Нижня перемичка)
    var lineIJ = Lines.First(p => p.Name == "IJ");
    // Прив'язуємо до точки E (ліва внутрішня) та піднімаємо на 12 мм (фіксовано або через параметр)
    // Але для простоти розташуємо її на початку внутрішніх ліній + 12 мм вгору
    double bottomOffset = 12 * PixelsInSm;
    
    I._x = E._x;
    I._y = A._y - bottomOffset; // Від низу вгору
    
    J._x = H._x;
    J._y = D._y - bottomOffset;
    
    lineIJ.P1 = I; lineIJ.P2 = J;

    // KL (Верхня перемичка)
    var lineKL = Lines.First(p => p.Name == "KL");
    // Розташуємо її на 12 мм знизу + 62 мм висоти вікна = 74 мм від низу
    double topOffset = (12 + 62) * PixelsInSm;

    K._x = E._x;
    K._y = A._y - topOffset;

    L._x = H._x;
    L._y = D._y - topOffset;

    lineKL.P1 = K; lineKL.P2 = L;
    
    // --- 4. Бічні лінії зліва (ті що червоні на скріншоті) ---
    
    // Нижня лінія (R -> I)
    // R лежить на лінії AB (ліва зовнішня), тому X як у A.
    // Y такий самий, як у точки I (нижня перемичка).
    double shift = 5 * PixelsInSm; 

    // Нижня червона лінія (R -> S)
    R._x = A._x;          // На зовнішній лінії
    R._y = I._y + shift;  // Вниз від внутрішньої перемички (плюс по Y = вниз на екрані)
    
    S._x = E._x;          // На внутрішній лінії
    S._y = R._y;          // Горизонтально

    var lineSideBot = Lines.First(p => p.Name == "SideBot");
    lineSideBot.P1 = R; lineSideBot.P2 = S;

    // Верхня червона лінія (M -> T)
    M._x = A._x;          // На зовнішній лінії
    M._y = K._y - shift * 2;  // Вниз від верхньої перемички
    
    T._x = E._x;          // На внутрішній лінії
    T._y = M._y;          // Горизонтально

    var lineSideTop = Lines.First(p => p.Name == "SideTop");
    lineSideTop.P1 = M; lineSideTop.P2 = T;
    
    T._x = M._x + lineSideTop.Distance * PixelsInSm; 
    T._y = M._y;          

    lineSideTop.P1 = M; lineSideTop.P2 = T;

    // --- 5. КОЛО (Ось ця частина у вас загубилася) ---
    
    // Знаходимо геометричний центр прямокутника
    double centerX = (A._x + D._x) / 2;
    double centerY = (A._y + B._y) / 2;

    // Задаємо точки N (лівий край) та P (правий край) відносно центру
    N._x = centerX - rSmall * PixelsInSm;
    N._y = centerY;

    P._x = centerX + rSmall * PixelsInSm;
    P._y = centerY;
    
    
}

    public static void ReCalculateAll()
    {
        foreach (var line in Lines)
        {
            line.CalculateDistance();
        }
    }
}