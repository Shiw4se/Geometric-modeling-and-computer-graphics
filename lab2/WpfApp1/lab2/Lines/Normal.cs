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
       
        double r = a * Math.Sin(k * _phi);
        
        double dr = a * k * Math.Cos(k * _phi);

        // 2. Похідні для Декартових координат
        
        double dx = dr * Math.Cos(_phi) - r * Math.Sin(_phi);
        
        double dy = dr * Math.Sin(_phi) + r * Math.Cos(_phi);

        // 3. Вектор дотичної: T = (dx, dy)
       
        double nx = -dy;
        double ny = dx;

        // 4. Нормалізація вектора (робимо його довжиною 1)
        double len = Math.Sqrt(nx * nx + ny * ny);
        
        
        if (len < 0.00001) return;

        nx /= len;
        ny /= len;

        // 5. Малюємо лінію на екрані
        double visualLength = 60; 
        double halfLen = visualLength / 2;

       
        double screenNx = nx;
        double screenNy = -ny;

        
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