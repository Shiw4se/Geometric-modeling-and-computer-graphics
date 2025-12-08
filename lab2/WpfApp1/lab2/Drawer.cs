using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace lab2;

public class Drawer
{
    private static Canvas _canvas;
    public double _pixelsInSm;
    private Ellipse _prevDot;
    private bool isAnimated = true;
    private Line _prevTangent;
    private Line _prevNormal;
    private int _counter;

    public Drawer(Canvas canvas, double pixelsInSm)
    {
        _canvas = canvas;
        _pixelsInSm = pixelsInSm;
        _counter = 0;
    }
    
    // ОНОВЛЕНИЙ МЕТОД ДЛЯ НОВОЇ СИСТЕМИ КООРДИНАТ
    public void DrawSystem()
    {
        // 1. Малюємо жирні осі координат (X та Y)
        // Використовуємо точки P_Left, P_Right, P_Up, P_Down з CoordinateSystem
        
        // Горизонтальна вісь (X)
        DrawLine(CoordinateSystem.P_Left, CoordinateSystem.P_Right, 2); 
        
        // Вертикальна вісь (Y)
        DrawLine(CoordinateSystem.P_Down, CoordinateSystem.P_Up, 2);    

        // 2. Малюємо сітку (тонкі сірі лінії)
        // Вертикальні лінії сітки
        for (int i = 0; i < CoordinateSystem.YLines.Count; i += 2)
        {
            DrawBackLine(CoordinateSystem.YLines[i], CoordinateSystem.YLines[i+1]);
        }

        // Горизонтальні лінії сітки
        for (int i = 0; i < CoordinateSystem.XLines.Count; i += 2)
        {
            DrawBackLine(CoordinateSystem.XLines[i], CoordinateSystem.XLines[i+1]);
        }
    }

    // --- Метод для малювання звичайної чорної лінії (для осей) ---
    public void DrawLine(Point pbeg, Point pend, double thickness = 1)
    {
        var ln = new System.Windows.Shapes.Line
        {
            Stroke = Brushes.Black,
            StrokeThickness = thickness,
            X1 = pbeg.X,
            Y1 = pbeg.Y,
            X2 = pend.X,
            Y2 = pend.Y
        };
        _canvas.Children.Add(ln);
    }
    
    // --- Метод для малювання сірої лінії (сітка) ---
    private void DrawBackLine(Point pbeg, Point pend)
    {
        var ln = new System.Windows.Shapes.Line
        {
            Stroke = Brushes.LightGray,
            StrokeThickness = 1,
            X1 = pbeg.X,
            Y1 = pbeg.Y,
            X2 = pend.X,
            Y2 = pend.Y
        };
        _canvas.Children.Add(ln);
    }

    // --- ТИМЧАСОВА ЗАГЛУШКА ДЛЯ DRAW MODEL ---
    // Поки що закоментуємо логіку анімації та кривих, 
    // оскільки Model ще не готова для Лаб 2.
    // Коли ви оновите Model.cs для малювання троянди, цей код треба буде розкоментувати і виправити.

    public void DrawModel()
    {
        _canvas.Children.Clear();
        DrawSystem();
        
        
        for (int i = 0; i < Model.Points.Count - 1; i++)
        {
             DrawLine(Model.Points[i], Model.Points[i+1], 2); // Малюємо сегменти кривої
        }
        // Малюємо дотичні та нормалі, якщо вони існують
        // (Це буде працювати, коли ви додасте логіку розрахунку дотичних у Model)
        if (Model.Tangents != null)
        {
            for (int i = 0; i < Model.Tangents.Count; i += 2)
            {
                if (Model.Tangents[i] != null && Model.Tangents[i+1] != null)
                    DrawAuxLine(Model.Tangents[i], Model.Tangents[i+1], Brushes.Blue);
            }
        }
        
        if (Model.Normals != null)
        {
            for (int i = 0; i < Model.Normals.Count; i += 2)
            {
                if (Model.Normals[i] != null && Model.Normals[i+1] != null)
                    DrawAuxLine(Model.Normals[i], Model.Normals[i+1], Brushes.Red);
            }
        }
    }

    private void DrawAuxLine(Point pbeg, Point pend, Brush color)
    {
        if (pbeg == null || pend == null) return;
        
        var ln = new Line
        {
            Stroke = color,
            StrokeThickness = 2,
            X1 = pbeg.X,
            Y1 = pbeg.Y,
            X2 = pend.X,
            Y2 = pend.Y
        };
        _canvas.Children.Add(ln);
    }
    
    

    public void PauseAnimation()
    {
        isAnimated = false;
    }
}