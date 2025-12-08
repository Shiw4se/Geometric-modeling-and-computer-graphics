using System.Windows;

namespace lab1;


public class Zoom
{
    public static void ZoomIn()
    {
        
    }

    public static void ZoomOut(double pixelInSm)
    {
        
        foreach (var point in View.Points)
        {
            var vector = new Vector();
            vector.X = point.X;
            vector.Y = point.Y;
            point._x = point.XToSystem() * pixelInSm + 100;
            point._y=700-point.YToSystem()*pixelInSm;
            
        }
        View.rSmall=Math.Sqrt(Math.Pow(View.N.X - View.P.X, 2) + Math.Pow(View.N.Y - View.P.Y, 2)) /
                    pixelInSm / 2/2;
    }
}