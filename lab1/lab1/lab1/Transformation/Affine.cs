using System.Windows;
using System.Windows.Media;

namespace lab1
{
    public static class Affine
    {
        public static void Transform(Point X, Point Y, Point O)
        {
            var matrix = new Matrix();
            matrix.M11 = X.X; matrix.M12 = X.Y;
            matrix.M21 = Y.X; matrix.M22 = Y.Y;
            matrix.OffsetX = O.X; matrix.OffsetY = O.Y;

            // 1. Фігура
            if (View.Points != null)
                foreach (var point in View.Points) if (point != null) ApplyMatrix(point, matrix);

            // 2. Коло
            if (View.Circle != null)
                foreach (var point in View.Circle) if (point != null) ApplyMatrix(point, matrix);

            // 3. СІТКА І ОСІ (Щоб все рухалось разом)
            
            // Головні точки осей (Чорні лінії)
            ApplyMatrix(CoordinateSystem.O, matrix);
            ApplyMatrix(CoordinateSystem.P1, matrix);
            ApplyMatrix(CoordinateSystem.P2, matrix);

            // Лінії сітки (Сірі лінії)
            if (CoordinateSystem.XLines != null)
                foreach (var point in CoordinateSystem.XLines) if (point != null) ApplyMatrix(point, matrix);

            if (CoordinateSystem.YLines != null)
                foreach (var point in CoordinateSystem.YLines) if (point != null) ApplyMatrix(point, matrix);
        }

        private static void ApplyMatrix(Point point, Matrix matrix)
        {
            if (point == null) return;

            var vector = new Vector();
            // Важливо: ми трансформуємо відносно поточного положення точки, 
            // тому XToSystem має брати поточні координати
            vector.X = point.XToSystem();
            vector.Y = point.YToSystem();

            // Формула: НоваКоордината = Зміщення + (Вектор * Матриця) * Масштаб + ПочатковийЗсувЕкрану
            // 50 і 750 - це ваші початкові Ox та Oy з Model/CoordinateSystem
            point._x = (matrix.OffsetX + vector.X * matrix.M11 + vector.Y * matrix.M21) * Model.PixelsInSm + 50;
            point._y = 750 - (matrix.OffsetY + vector.X * matrix.M12 + vector.Y * matrix.M22) * Model.PixelsInSm;
        }
    }
}