using System;
using System.Windows;
using System.Windows.Media;

namespace lab2;

public class Euclid
{
    public static void Move(double x, double y, double px)
    {
        // Рухаємо основні точки кривої
        if (Model.Points != null)
        {
            foreach (var point in Model.Points)
            {
                point.X += x * px;
                point.Y -= y * px; // Y інвертований на екрані
            }
        }

        // Рухаємо нормалі (якщо вони існують)
        if (Model.Normals != null)
        {
            foreach (var point in Model.Normals)
            {
                point.X += x * px;
                point.Y -= y * px;
            }
        }

        // Рухаємо дотичні (якщо вони існують)
        if (Model.Tangents != null)
        {
            foreach (var point in Model.Tangents)
            {
                point.X += x * px;
                point.Y -= y * px;
            }
        }
    }

    public static void Rotate(Point p, double angle, double px)
    {
        var matrix = new Matrix();
        
        // Переводимо кут в радіани
        double rad = angle * (Math.PI / 180);

        matrix.M11 = Math.Cos(rad);
        matrix.M12 = Math.Sin(rad);
        matrix.M21 = -Math.Sin(rad);
        matrix.M22 = Math.Cos(rad);
        
        // Розрахунок зміщення відносно точки повороту p
        matrix.OffsetX = (0 - p.X) * (matrix.M11 - 1) + p.Y * matrix.M12;
        matrix.OffsetY = (0 - p.X) * matrix.M12 - p.Y * (matrix.M11 - 1);

        // Повертаємо основні точки кривої
        if (Model.Points != null)
        {
            foreach (var point in Model.Points)
            {
                ApplyRotation(point, matrix);
            }
        }

        // Повертаємо нормалі
        if (Model.Normals != null)
        {
            foreach (var point in Model.Normals)
            {
                ApplyRotation(point, matrix);
            }
        }

        // Повертаємо дотичні
        if (Model.Tangents != null)
        {
            foreach (var point in Model.Tangents)
            {
                ApplyRotation(point, matrix);
            }
        }
    }

    private static void ApplyRotation(Point point, Matrix matrix)
    {
        var vector = new Vector();
        vector.X = point.X;
        vector.Y = point.Y;
        
        point.X = matrix.M11 * vector.X + matrix.M21 * vector.Y + 1 * matrix.OffsetX;
        point.Y = matrix.M12 * vector.X + matrix.M22 * vector.Y + 1 * matrix.OffsetY;
    }
}