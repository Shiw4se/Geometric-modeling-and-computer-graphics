using System.Windows;
using System.Windows.Media;

namespace lab1;

    


public class Symmetry
{
    public static void Transform(Point p)
    {
        var matrix = new Matrix();
        matrix.M11=-1;
        matrix.M12=0;
        matrix.M21=0;
        matrix.M22=-1;
        matrix.OffsetX=2*p.X;
        matrix.OffsetY =2*p.Y;
        foreach (var point in Model.Points)
        {
            var vector = new Vector();
            vector.X = point.X;
            vector.Y = point.Y;
            point._x=matrix.M11*vector.X+matrix.M21*vector.Y+1*matrix.OffsetX;
            point._y=matrix.M12*vector.X+matrix.M22*vector.Y+1*matrix.OffsetY;
        }
        
    }
}