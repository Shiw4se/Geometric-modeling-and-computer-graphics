namespace lab3;

public class Point
{
    // Приватні поля для зберігання координат
    private double _x;
    private double _y;
    
    // Властивості для доступу (з великої літери)
    public double X
    {
        get => _x;
        set { _x = value; }
    }

    public double Y
    {
        get => _y;
        set { _y = value; }
    }

    // Інші властивості
    public string PointName { get; set; }
    public bool isControl { get; set; } // true = Керуюча (зелена), false = Вузлова (червона)
    public bool isNode { get; set; }    // (Можна використовувати для позначення стиків)
    
    // Нове поле для Лабораторної №4 (Гладкість)
    public bool isSmooth { get; set; } = true; // true = гладкий стик, false = злам

    public bool IsBreak;
    // Конструктори
    public Point()
    {
        X = 0;
        Y = 0;
        PointName = "Undefined";
    }

    public Point(double x, double y, string name = "", bool isControl = false)
    {
        X = x;
        Y = y;
        PointName = name;
        this.isControl = isControl;
        this.isNode = !isControl; // Якщо не керуюча, то вузлова
    }

    // Метод клонування (корисний для анімації та бекапів)
    public Point Clone()
    {
        return new Point(X, Y, PointName, isControl) { isNode = this.isNode, isSmooth = this.isSmooth };
    }
}