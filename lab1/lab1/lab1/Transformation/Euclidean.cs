using System.Windows.Media;
using System.Windows;


namespace lab1
{
    public static class Euclidean
    {
        public static void Move(double n,double px)
        {
            foreach (var point in Model.Points)
            {
                point._x =point.X+n*px;
                point._y =point.Y-n*px;
            }
        }

        public static void Rotate(Point p, double angle, double px)
        {
            var matrix = new Matrix();
            matrix.M11 = Math.Cos(angle * (Math.PI / 180));
            matrix.M12 = Math.Sin(angle * (Math.PI / 180));
            matrix.M21 = -Math.Sin(angle * (Math.PI / 180));
            matrix.M22 = Math.Cos(angle * (Math.PI / 180));
            matrix.OffsetX = (0 - p.X) * (matrix.M11 - 1) + p.Y * matrix.M12;
            matrix.OffsetY = (0 - p.X) * matrix.M12 - p.Y * (matrix.M11 - 1);
            foreach (var point in Model.Points)
            {
                var vector = new Vector();
                vector.X = point.X;
                vector.Y = point.Y;
                point._x = matrix.M11 * vector.X + matrix.M21 * vector.Y + 1 * matrix.OffsetX;
                point._y = matrix.M12 * vector.X + matrix.M22 * vector.Y + 1 * matrix.OffsetY;

            }
        }
    }
}