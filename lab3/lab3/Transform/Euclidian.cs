using System;
using System.Windows;
using System.Windows.Media;

namespace lab3;

public class Euclidian
{
    public static void Move(double x, double y, double px, Model model)
    {
        // 1. Рухаємо точки каркасу
        foreach (var point in model.Points)
        {
            point.X += x * px;
            point.Y -= y * px; // Y інвертований на екрані (вниз - це плюс)
        }
        
        // 2. Оновлюємо саму криву на основі нових точок каркасу
        model.CreateCurvePoints();
    }

    public static void Rotate(Point center, double angle, Model model)
    {
        var matrix = new Matrix();
        
        // Переведення кута в радіани
        double rad = angle * (Math.PI / 180.0);

        matrix.M11 = Math.Cos(rad);
        matrix.M12 = Math.Sin(rad);
        matrix.M21 = -Math.Sin(rad);
        matrix.M22 = Math.Cos(rad);
        
        // Розрахунок зміщення для повороту навколо точки center
        // Формула: x' = xc + (x-xc)cos - (y-yc)sin
        // Матриця WPF робить це трохи інакше, тому ми налаштовуємо Offset
        matrix.OffsetX = (0 - center.X) * (matrix.M11 - 1) + center.Y * matrix.M12;
        matrix.OffsetY = (0 - center.X) * matrix.M12 - center.Y * (matrix.M11 - 1);

        // 1. Повертаємо точки каркасу
        foreach (var point in model.Points)
        {
            var vector = new Vector();
            vector.X = point.X;
            vector.Y = point.Y;
            
            // Застосування матриці
            point.X = matrix.M11 * vector.X + matrix.M21 * vector.Y + 1 * matrix.OffsetX;
            point.Y = matrix.M12 * vector.X + matrix.M22 * vector.Y + 1 * matrix.OffsetY;
        }
        
        // 2. Перераховуємо криву для нового положення каркасу
        model.CreateCurvePoints();
    }
}