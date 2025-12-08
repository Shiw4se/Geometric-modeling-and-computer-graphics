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
        // r = a * sin(k * phi)
        double r = a * Math.Sin(k * _phi);
        // dr/dphi = a * k * cos(k * phi)
        double dr = a * k * Math.Cos(k * _phi);

        // 2. Похідні для Декартових координат (x', y')
        // x' = dr * cos(phi) - r * sin(phi)
        double dx = dr * Math.Cos(_phi) - r * Math.Sin(_phi);
        // y' = dr * sin(phi) + r * cos(phi)
        double dy = dr * Math.Sin(_phi) + r * Math.Cos(_phi);

        // 3. Вектор дотичної: T = (dx, dy)
        // На відміну від нормалі, тут ми беремо вектор прямо як є.
        
        // Нормалізація вектора (робимо довжину = 1)
        double len = Math.Sqrt(dx * dx + dy * dy);
        
        // Захист від ділення на нуль (наприклад, у точках перегину або центрі)
        if (len < 0.00001) return;

        double tx = dx / len;
        double ty = dy / len;

        // 4. Малюємо лінію на екрані
        double visualLength = 60; // Довжина дотичної в пікселях
        double halfLen = visualLength / 2;

        // Врахування екранних координат:
        // Математичний Y росте вгору, Екранний Y росте вниз.
        // Тому вектор (tx, ty) на екрані перетворюється на (tx, -ty).
        double screenTx = tx;
        double screenTy = -ty;

        // Будуємо відрізок навколо точки _point
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