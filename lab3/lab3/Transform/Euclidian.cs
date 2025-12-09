using System;
using System.Windows;
using System.Windows.Media;

namespace lab3;

public class Euclidian
{
    // Метод переміщення (Move)
    public static void Move(double x, double y, double px, Model model)
    {
        // 1. Рухаємо кожну точку каркасу
        foreach (var point in model.Points)
        {
            // Додаємо зміщення (переводимо умовні одиниці в пікселі)
            point.X += x * px;
            point.Y -= y * px; // Y інвертований (вниз - це плюс), тому віднімаємо для руху вгору
        }
        
        // 2. ВАЖЛИВО: Оновлюємо саму криву на основі нових позицій каркасу
        model.CreateCurvePoints();
    }

    // Метод обертання (Rotate)
    public static void Rotate(Point center, double angle, Model model)
    {
        var matrix = new Matrix();
        
        // Переводимо кут з градусів у радіани
        double rad = angle * (Math.PI / 180.0);

        // Заповнюємо матрицю повороту
        matrix.M11 = Math.Cos(rad);
        matrix.M12 = Math.Sin(rad);
        matrix.M21 = -Math.Sin(rad);
        matrix.M22 = Math.Cos(rad);
        
        // Розрахунок зміщення (Offset) для повороту навколо конкретної точки center.
        // Це математичний трюк: ми переносимо центр в (0,0), крутимо, і переносимо назад.
        // Формула для WPF Matrix:
        matrix.OffsetX = (0 - center.X) * (matrix.M11 - 1) + center.Y * matrix.M12;
        matrix.OffsetY = (0 - center.X) * matrix.M12 - center.Y * (matrix.M11 - 1);

        // 1. Застосовуємо матрицю до кожної точки каркасу
        foreach (var point in model.Points)
        {
            var vector = new Vector();
            vector.X = point.X;
            vector.Y = point.Y;
            
            // Множення вектора на матрицю
            point.X = matrix.M11 * vector.X + matrix.M21 * vector.Y + 1 * matrix.OffsetX;
            point.Y = matrix.M12 * vector.X + matrix.M22 * vector.Y + 1 * matrix.OffsetY;
        }
        
        // 2. Перераховуємо гладку криву для нового положення каркасу
        model.CreateCurvePoints();
    }
}