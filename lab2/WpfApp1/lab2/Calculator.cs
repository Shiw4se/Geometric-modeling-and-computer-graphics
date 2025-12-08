using System;
using System.Collections.Generic;

namespace lab2;

public static class Calculator
{
    /// <summary>
    /// Розрахунок довжини кривої методом суми відрізків між точками.
    /// </summary>
    public static double CalculateLineLength()
    {
        double sum = 0;
        
        // Перевіряємо, чи список точок не порожній
        if (Model.Points != null && Model.Points.Count > 1)
        {
            for (int i = 0; i < Model.Points.Count - 1; i++)
            {
                var p1 = Model.Points[i];
                var p2 = Model.Points[i + 1];
                
                // Відстань між сусідніми точками за теоремою Піфагора
                // Ділимо на PixelsInSm, щоб отримати результат в умовних одиницях (сантиметрах), а не пікселях
                double dist = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
                
                sum += dist / Model.PixelsInSm; 
            }
        }

        return sum;
    }

    /// <summary>
    /// Розрахунок площі фігури (аналітична формула для Троянди).
    /// </summary>
    public static double CalculateSquare()
    {
        double k = Model.K;
        
        // Радіус 'a' беремо з моделі. 
        // Якщо хочете площу в см², треба поділити A на PixelsInSm. 
        // Якщо A задано просто як параметр (наприклад, 100), то рахуємо в "умовних одиницях".
        double a = Model.A / Model.PixelsInSm; 

        // Перевірка на ціле число (з невеликою похибкою для double)
        bool isInteger = Math.Abs(k % 1) < 0.001;
        
        if (isInteger)
        {
            // Формула:
            // Якщо K парне (4, 6... -> пелюсток 2k): S = (PI * a^2) / 2
            if (Math.Abs(k % 2) < 0.001) 
            {
                return (Math.PI * Math.Pow(a, 2)) / 2.0;
            }
            // Якщо K непарне (3, 5... -> пелюсток k): S = (PI * a^2) / 4
            else 
            {
                return (Math.PI * Math.Pow(a, 2)) / 4.0;
            }
        }
        else
        {
            // Для дробових k точна формула дуже складна, повертаємо 0 (або можна наблизити)
            return 0; 
        }
    }

    /// <summary>
    /// Розрахунок точок перегину.
    /// </summary>
    public static List<Point> CalculateInflectionsPoints()
    {
        // Для троянди (r = a*sin(k*phi)) точок перегину в класичному розумінні немає,
        // тому повертаємо порожній список.
        return new List<Point>();
    }
}