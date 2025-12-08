namespace lab1;

public class Point
{
    protected internal double _x;
    protected internal double _y;
    public double X
    {
        get => _x;
        set
        {
            if ((int)X != (int)value)
            {
                _x = value;
                foreach (var line in Model.Lines.FindAll(p => p.P1 == this || p.P2 == this))
                {
                    line.CalculateDistance();

                }
            }
        }
    } 
    public double Y
    {
        get => _y;
        set
        {
            if((int)_y != (int)value)
            {
                _y = value;
                foreach (var line in Model.Lines.FindAll(p => p.P1 == this || p.P2 == this))
                { 
                    line.CalculateDistance();
                        
                }
            }
        } 
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

    public double XToSystem() => (X - 50) / Model.PixelsInSm; 
    

    public double YToSystem()=> (750-Y) / Model.PixelsInSm;

    public Point Clone()
    {
        return new Point(X, Y);
    }
}