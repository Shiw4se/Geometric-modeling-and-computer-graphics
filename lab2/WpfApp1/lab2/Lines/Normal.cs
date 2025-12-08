using System;

namespace lab2;

public class Normal
{
    private Point _point; // Точка на екрані
    private double _phi;  // Кут (параметр)
    
    public Normal(Point p, double phi)
    {
        _point = p;
        _phi = phi;
    }

    public void CalculateNormal()
    {
        double a = Model.A;
        double k = Model.K; // Переконайтеся, що в Model є поле K

        // 1. Полярна формула та її похідна
        // r = a * sin(k * phi)
        double r = a * Math.Sin(k * _phi);
        // dr/dphi = a * k * cos(k * phi)
        double dr = a * k * Math.Cos(k * _phi);

        // 2. Похідні для Декартових координат
        // x' = dr * cos(phi) - r * sin(phi)
        double dx = dr * Math.Cos(_phi) - r * Math.Sin(_phi);
        // y' = dr * sin(phi) + r * cos(phi)
        double dy = dr * Math.Sin(_phi) + r * Math.Cos(_phi);

        // 3. Вектор дотичної: T = (dx, dy)
        // Вектор нормалі перпендикулярний до дотичної: N = (-dy, dx)
        double nx = -dy;
        double ny = dx;

        // 4. Нормалізація вектора (робимо його довжиною 1)
        double len = Math.Sqrt(nx * nx + ny * ny);
        
        // Захист від ділення на нуль (наприклад, у центрі)
        if (len < 0.00001) return;

        nx /= len;
        ny /= len;

        // 5. Малюємо лінію на екрані
        double visualLength = 60; // Довжина нормалі в пікселях
        double halfLen = visualLength / 2;

        // Врахування екранних координат:
        // Математичний Y росте вгору, Екранний Y росте вниз.
        // Тому вектор (nx, ny) на екрані стає (nx, -ny).
        double screenNx = nx;
        double screenNy = -ny;

        // Будуємо відрізок навколо точки _point
        var start = new Point(
            _point.X - halfLen * screenNx,
            _point.Y - halfLen * screenNy
        );

        var end = new Point(
            _point.X + halfLen * screenNx,
            _point.Y + halfLen * screenNy
        );

        Model.Normals.Add(start);
        Model.Normals.Add(end);
    }
}