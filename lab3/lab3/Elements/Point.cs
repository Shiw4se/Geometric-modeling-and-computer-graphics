namespace lab3;

public class Point
{
    private double _x;
    private double _y;
    public string PointName;
    public bool isControl;
    public bool isNode;

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

    public Point()
    {
        X = 0;
        Y = 0;
        PointName = "Undefined";
        isControl = false;
        isNode = false;
    }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
        PointName = "Undefined";
        isControl = false;
        isNode = false;
    }

    public Point(double x, double y, string name, bool control)
    {
        X = x;
        Y = y;
        PointName = name;
        isControl = control;
        isNode = false;
    }

    public Point(double x, double y, string name)
    {
        X = x;
        Y = y;
        PointName = name;
        isControl = false;
        isNode = false;
    }

    public double XToSystem() => (X - Coordinate.O.X) / Model.PixelsInSm;

    public double YToSystem() => (Coordinate.O.Y - Y) / Model.PixelsInSm;

    public void TransferToCanvas()
    {
        X = _x * Model.PixelsInSm + Coordinate.O.X;
        Y = Coordinate.O.Y - _y * Model.PixelsInSm;
    }

    public Point Clone()
    {
        return new Point(X, Y);
    }
}