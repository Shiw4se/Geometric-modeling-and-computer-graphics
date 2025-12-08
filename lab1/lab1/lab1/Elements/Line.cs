using System.Windows.Media;
using System.Windows.Shapes;

namespace lab1
{
    public class Line
    {
       public String Name;
    public Point P1, P2;
    private double _distance;
    public double DistanceOld;
    public double Distance
    {
        get => _distance;
        set
        {

            if(Math.Abs(value - _distance) > 0.0001 && Math.Abs(value - DistanceOld) > 0.0001)
            {
                double[] unitVector = CalculateUnitVector(P1.X, P1.Y, P2.X, P2.Y, _distance);
                // Calculate the midpoint M
                (double mx, double my) = ((P1.X + P2.X) / 2, (P1.Y + P2.Y) / 2);

                // Calculate the new positions for A' and B'
                double halfNewLength = value / 2;
                (double newX1, double newY1) = (mx - halfNewLength * unitVector[0], my - halfNewLength * unitVector[1]);
                (double newX2, double newY2) = (mx + halfNewLength * unitVector[0], my + halfNewLength * unitVector[1]);
                var Lines = Model.Lines;
                P1.X= newX1;
                P1.Y = newY1;
                P2.X = newX2;
                P2.Y = newY2;
 
                DistanceOld = _distance;
                _distance = value;
                
                
            }
        }
    }

    public Line(String name, Point p1, Point p2, double length)
    {
        Name = name;
        P1 = p1;
        P2 = p2;
        _distance = length;
        DistanceOld = _distance;
    }
    public void CalculateDistance()
    {
        if (Name != "NPR")
        {
            _distance=Math.Sqrt(Math.Pow(P2.X - P1.X, 2) + Math.Pow(P2.Y - P1.Y, 2))/Model.PixelsInSm;
        }else
        {_distance=Math.Sqrt(Math.Pow(P2.X - P1.X, 2) + Math.Pow(P2.Y - P1.Y, 2))/Model.PixelsInSm/2;}
         
    }

    static double[] CalculateUnitVector(double x1, double y1, double x2, double y2, double length)
    {
        double[] unitVector = new double[2];
        unitVector[0] = (x2 - x1) / length;
        unitVector[1] = (y2 - y1) / length;
        return unitVector;
    }
    
    
    }
}