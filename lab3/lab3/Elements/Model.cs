using System;
using System.Collections.Generic;

namespace lab3;

public class Model
{
    public List<Point> Points;
    public List<Point> CurvePoints;
    
    public static double PixelsInSm { get; set; }
    public static double Ox, Oy;

    // Параметр ваги для раціональної кривої (важливо для другого методу)
    public static double HyperbolaWeight = 2.0; 

    public Model(double pix)
    {
        Points = new List<Point>();
        PixelsInSm = pix;
        
        CreatePoints();      // Генеруємо каркас пальми
        CreateCurvePoints(); // Рахуємо гладку лінію (другий метод)
    }

    // Метод побудови точок (Пальма)
    private void CreatePoints()
    {
        Points.Clear();

        // Центр "корони" пальми (верхівка стовбура)
        double cx = 715; 
        double cy = 380;

        // ==========================================
        // 1. ОСТРІВ (Трохи підправив під референс)
        // ==========================================
        // Лівий край (рваний)
        AddSegment(150, 780,  200, 760,  220, 780); 
        AddSegment(220, 780,  240, 750,  280, 770); 
        // Основний пагорб
        AddSegment(280, 770,  350, 680,  450, 700);
        AddSegment(450, 700,  500, 720,  550, 750); 
        // Правий хвіст
        AddSegment(780, 750,  900, 730,  1000, 750); 
        AddSegment(1000, 750, 1020, 760, 1030, 800); 
        // Низ острова
        AddSegment(1030, 800, 800, 810,  600, 805);
        AddSegment(600, 805,  400, 810,  150, 780); 

        // ==========================================
        // 2. СТОВБУР (Вигнутий як на референсі)
        // ==========================================
        // Ліва сторона (дуга вправо)
        AddSegment(550, 750,  650, 600,  680, 500); 
        AddSegment(680, 500,  700, 420,  cx, cy);

        // Права сторона (паралельна дуга)
        // Повертаємось від центру вниз
        AddSegment(cx + 15, cy + 10, 730, 480, 700, 600);
        AddSegment(700, 600,  680, 700, 780, 750); 


        // ==========================================
        // 3. ЛИСТЯ (ТЕПЕР ОБ'ЄМНЕ З ЗУБЧИКАМИ)
        // ==========================================
        
        // --- ЛИСТОК 1 (Лівий Верхній - стирчить вліво) ---
        // Верхня грань (гладка)
        AddSegment(cx, cy,    650, 300,  580, 280); 
        // Кінчик листка
        AddSegment(580, 280,  560, 275,  540, 300); 
        // Нижня грань (ЗУБЧИКИ назад до центру)
        AddSegment(540, 300,  570, 310,  590, 330); // Зуб 1
        AddSegment(590, 330,  610, 310,  630, 340); // Зуб 2
        AddSegment(630, 340,  660, 350,  cx, cy);   // До стовбура

        // --- ЛИСТОК 2 (Лівий Нижній - звисає вниз) ---
        // Верхня грань
        AddSegment(cx, cy,    640, 380,  580, 400);
        AddSegment(580, 400,  550, 420,  520, 460); // Кінчик вниз
        // Повернення (внутрішня рвана частина)
        AddSegment(520, 460,  540, 430,  560, 440); // Зубчик
        AddSegment(560, 440,  580, 410,  610, 430); // Зубчик
        AddSegment(610, 430,  650, 410,  cx, cy);

        // --- ЛИСТОК 3 (Правий Великий - "парасолька") ---
        // Це найбільший листок, що накриває пальму зверху
        // Верхня дуга
        AddSegment(cx, cy,    750, 200,  850, 180);
        AddSegment(850, 180,  920, 200,  980, 280); // Кінчик справа внизу
        // Нижня грань (повертаємось "хвилями")
        AddSegment(980, 280,  940, 250,  900, 290); // Великий виріз
        AddSegment(900, 290,  880, 240,  840, 270); // Менший виріз
        AddSegment(840, 270,  800, 300,  cx, cy);   // Замикаємо

        // --- ЛИСТОК 4 (Правий Нижній - короткий) ---
        // Верхня грань
        AddSegment(cx, cy,    780, 350,  820, 380);
        // Кінчик звисає
        AddSegment(820, 380,  840, 400,  850, 450);
        // Нижня грань (повернення)
        AddSegment(850, 450,  820, 420,  800, 440); // Зубчик
        AddSegment(800, 440,  780, 400,  cx, cy);
        
        // --- ЛИСТОК 5 (Маленький чубчик по центру) ---
        // Стирчить прямо вгору, закриває стик
        AddSegment(cx, cy,    700, 300,  720, 250); // Вгору лівіше
        AddSegment(720, 250,  730, 300,  cx, cy);   // Вниз
    }

    // Допоміжний метод для додавання трійки точок
    // Адаптовано під конструктор Point(x, y, name, isControl)
    private void AddSegment(double x1, double y1, double x2, double y2, double x3, double y3)
    {
        // P(i) - звичайна точка
        Points.Add(new Point(x1, y1, "Start"));          
        // P(i+1) - контрольна точка (тягне криву на себе)
        Points.Add(new Point(x2, y2, "Control", true)); 
        // P(i+2) - звичайна точка
        Points.Add(new Point(x3, y3, "End"));          
    }

    // Метод розрахунку кривої (Другий метод: Раціональна крива Безьє)
    public void CreateCurvePoints()
    {
        CurvePoints = new List<Point>();
        double w = HyperbolaWeight; 

        // УВАГА: Тут крок циклу i += 3, тому що ми додаємо точки трійками через AddSegment
        for (int i = 0; i < Points.Count - 2; i += 3)
        {
            double P0x = Points[i].X;     double P0y = Points[i].Y;
            double P1x = Points[i + 1].X; double P1y = Points[i + 1].Y; // Контрольна
            double P2x = Points[i + 2].X; double P2y = Points[i + 2].Y;

            // Будуємо сегмент кривої
            for (double u = 0; u <= 1; u += 0.02) // Крок 0.02 для плавності
            {
                // Формула раціональної кривої Безьє 2-го порядку
                double poly0 = Math.Pow(1 - u, 2);
                double poly1 = 2 * u * (1 - u) * w; // Тут враховується вага w
                double poly2 = Math.Pow(u, 2);

                double denominator = poly0 + poly1 + poly2;

                double x = (poly0 * P0x + poly1 * P1x + poly2 * P2x) / denominator;
                double y = (poly0 * P0y + poly1 * P1y + poly2 * P2y) / denominator;

                CurvePoints.Add(new Point(x, y));
            }
            
            // Додаємо останню точку сегмента, щоб уникнути розривів при малюванні
            CurvePoints.Add(new Point(P2x, P2y)); 
        }
    }

    public static double DegToRad(double deg)
    {
        return deg * Math.PI / 180;
    }
}