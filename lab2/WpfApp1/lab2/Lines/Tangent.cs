using System;

namespace lab2;

public class Tangent
{
    private Point _point; // Точка на екрані
    private double _phi;  // Кут (параметр)

    public Tangent(Point p, double phi)
    {
        _point = p;
        _phi = phi;
    }

    public void CalculateTangent()
    {
        double a = Model.A;
        double k = Model.K; // Коефіцієнт пелюсток

        // 1. Полярна формула та її похідна
        
        double r = a * Math.Sin(k * _phi);
        
        double dr = a * k * Math.Cos(k * _phi);

        // 2. Похідні для Декартових координат (x', y')
        
        double dx = dr * Math.Cos(_phi) - r * Math.Sin(_phi);
        
        double dy = dr * Math.Sin(_phi) + r * Math.Cos(_phi);

        // 3. Вектор дотичної: T = (dx, dy)
        
        
        
        double len = Math.Sqrt(dx * dx + dy * dy);
        
       
        if (len < 0.00001) return;

        double tx = dx / len;
        double ty = dy / len;

        // 4. Малюємо лінію на екрані
        double visualLength = 60; 
        double halfLen = visualLength / 2;

        
        double screenTx = tx;
        double screenTy = -ty;

        
        var start = new Point(
            _point.X - halfLen * screenTx,
            _point.Y - halfLen * screenTy
        );

        var end = new Point(
            _point.X + halfLen * screenTx,
            _point.Y + halfLen * screenTy
        );

        Model.Tangents.Add(start);
        Model.Tangents.Add(end);
    }
}