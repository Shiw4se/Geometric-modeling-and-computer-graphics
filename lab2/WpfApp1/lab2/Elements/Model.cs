using System;
using System.Collections.Generic;

namespace lab2;

public class Model
{
    // Список точок для малювання кривої
    public static List<Point> Points; 
    
    public static double PixelsInSm { get; set; }
    
    // Словник для точного доступу до точок за кутом (для дотичних/нормалей)
    public static Dictionary<double, Point> Curves;
    
    // Списки для дотичних і нормалей (пари точок: початок-кінець)
    public static List<Point> Tangents;
    public static List<Point> Normals;
    public static List<Point> Inflections;
    
    public static double Length, Square;
    
    // Параметри троянди
    public static double A; // Радіус пелюстки (масштаб)
    public static double K; // Коефіцієнт (кількість пелюсток)

    public Model(double pix, double a, double k)
    {
        Points = new List<Point>();
        Tangents = new List<Point>();
        Normals = new List<Point>();
        Curves = new Dictionary<double, Point>();
        Inflections = new List<Point>();
        
        PixelsInSm = pix;
        A = a;
        K = k;
        
        Length = 0;
        
        CreatePoints();
        
        // Поки що закоментуємо, щоб не падало, якщо класи Calculator/Tangent ще не оновлені
        // CreateTangents();
        // CreateNormals();
        // Inflections = Calculator.CalculateInflectionsPoints();
        // Length = Calculator.CalculateLineLength();
        // Square = Calculator.CalculateSquare();
    }

    private void CreatePoints()
    {
        Points.Clear();
        Curves.Clear();

        // Крок кута (чим менше, тим гладша крива)
        var step = 0.01;
        
        // Межа циклу:
        // Якщо K ціле непарне -> PI (або 2*PI для надійності)
        // Якщо K ціле парне -> 2*PI
        // Якщо K дробове -> може знадобитися набагато більше (наприклад 10*PI або більше)
        // Для універсальності візьмемо 20*PI (гарантовано замкнеться для більшості простих дробів)
        // Але для початку (щоб не гальмувало) візьмемо 4*PI, якщо K ціле - можна менше.
        
        double maxPhi = 2 * Math.PI;
        
        // Перевірка на дробове K (проста евристика)
        if (K % 1 != 0) maxPhi = 12 * Math.PI; 
        else if (K % 2 == 0) maxPhi = 2 * Math.PI; // Парне
        else maxPhi = Math.PI; // Непарне (можна і 2PI, просто пройде двічі)

        for (double phi = 0; phi <= maxPhi; phi += step)
        {
            // --- ФОРМУЛА ТРОЯНДИ ---
            // r = a * sin(k * phi)
            // (можна використовувати cos, це просто поверне квітку)
            
            double r = A * Math.Sin(K * phi) * PixelsInSm; // Множимо на масштаб

            // Перехід в Декартові координати + зсув до центру екрану
            // x = r * cos(phi) + Ox
            // y = r * sin(phi) + Oy
            // У комп'ютерній графіці Y часто інвертований (мінус), 
            // але для полярних це просто змінить напрямок обходу.
            // Зробимо класично: Y росте вниз, тому "-" щоб Y ріс вгору візуально
            
            var x = r * Math.Cos(phi) + CoordinateSystem.O.X;
            var y = CoordinateSystem.O.Y - r * Math.Sin(phi); 

            var point = new Point(x, y);
            
            Points.Add(point);
            
            // Зберігаємо в словник (округлюємо ключ, щоб не було проблем з double)
            if (!Curves.ContainsKey(phi))
            {
                Curves.Add(phi, point);
            }
        }
    }

    /* Ці методи поки закоментуємо або залишимо пустими, 
       бо логіку розрахунку дотичних треба переписати під нову формулу.
    */
    private void CreateTangents() { }
    private void CreateNormals() { }
}


