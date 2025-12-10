using System;
using System.Windows;
using System.Windows.Media;

namespace lab3;

public class Euclidian
{
    // Метод переміщення (Move)
    public static void Move(double dx, double dy, Model model)
    {
        foreach (var p in model.Points)
        {
            p.X += dx;
            p.Y += dy;
        }

        model.CreateCurvePoints();
    }


    // Метод обертання (Rotate)
    public static void Rotate(Point center, double angle, Model model)
    {
        Matrix m = Matrix.Identity;
        m.RotateAt(angle, center.X, center.Y);

        foreach (var p in model.Points)
        {
            var np = m.Transform(new System.Windows.Point(p.X, p.Y));
            p.X = np.X;
            p.Y = np.Y;
        }

        model.CreateCurvePoints();
    }


}