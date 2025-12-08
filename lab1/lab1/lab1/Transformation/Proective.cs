using System;
using System.Windows;

namespace lab1
{
    public static class Proective
    {
        public static void Transform(Point X, Point Y, Point O, double wx, double wy, double wo)
        {
            // 1. Фігура
            if (View.Points != null)
                foreach (var point in View.Points) if (point != null) ApplyProjective(point, X, Y, O, wx, wy, wo);

            // 2. Коло
            if (View.Circle != null)
                foreach (var point in View.Circle) if (point != null) ApplyProjective(point, X, Y, O, wx, wy, wo);

            // 3. СІТКА І ОСІ
            ApplyProjective(CoordinateSystem.O, X, Y, O, wx, wy, wo);
            ApplyProjective(CoordinateSystem.P1, X, Y, O, wx, wy, wo);
            ApplyProjective(CoordinateSystem.P2, X, Y, O, wx, wy, wo);

            if (CoordinateSystem.XLines != null)
                foreach (var point in CoordinateSystem.XLines) if (point != null) ApplyProjective(point, X, Y, O, wx, wy, wo);

            if (CoordinateSystem.YLines != null)
                foreach (var point in CoordinateSystem.YLines) if (point != null) ApplyProjective(point, X, Y, O, wx, wy, wo);
        }

        private static void ApplyProjective(Point point, Point X, Point Y, Point O, double wx, double wy, double wo)
        {
            if (point == null) return;

            var vector = new Vector();
            vector.X = point.XToSystem();
            vector.Y = point.YToSystem();

            double denominator = wo + wx * vector.X + wy * vector.Y;
            if (Math.Abs(denominator) < 0.00001) denominator = 0.00001;

            point._x = ((O.X * wo + X.X * wx * vector.X + Y.X * wy * vector.Y) / denominator) * Model.PixelsInSm + 50;
            point._y = 750 - (O.Y * wo + X.Y * wx * vector.X + Y.Y * wy * vector.Y) / denominator * Model.PixelsInSm;
        }
    }
}