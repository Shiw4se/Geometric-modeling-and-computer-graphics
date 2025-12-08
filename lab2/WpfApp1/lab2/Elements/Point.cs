namespace lab2;

public class Point
{
    private double _x;
    private double _y;

    public double X
    {
        get => _x;
        set { _x = value; }
    }

    public double Y
    {
        get => _y;
        set
        { _y = value; }
    }

    public Point()
    {
        _x = 0;
        _y = 0;
    }

    public Point(double x, double y)
    {
        _x = x;
        _y = y;
    }

    public double XToSystem() => (X - CoordinateSystem.O.X) / Model.PixelsInSm;
    
    public double YToSystem() => (CoordinateSystem.O.Y - Y) / Model.PixelsInSm;

    public void TransferToCanvas()
    {
        X=_x*Model.PixelsInSm+CoordinateSystem.O.X;
        Y=CoordinateSystem.O.Y-_y*Model.PixelsInSm;
    }

    public Point Clone()
    {
        return new Point(X, Y);
    }
}