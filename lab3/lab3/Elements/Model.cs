using System;
using System.Collections.Generic;

namespace lab3
{
    public class Model
    {
        public List<Point> Points = new();
        public List<Point> CurvePoints = new();

        public static double HyperbolaWeight = 2.0;

        public Model(double pix)
        {
            CreatePoints();
            CreateCurvePoints();
        }

        // ------------------ Допоміжні конструктори точок ------------------

        private static Point P(double x, double y, bool isControl = false)
            => new Point(x, y, "", isControl);

        private void AddSegment(Point p0, Point p1, Point p2)
        {
            Points.Add(p0);
            Points.Add(p1);
            Points.Add(p2);
        }

        // ========================== Геометрія сцени ==========================

        private void CreatePoints()
        {
            Points.Clear();

            double cx = 715;
            double cy = 380;

            // Спільний центр крони (одна і та сама точка для всіх листків)
            var center = P(cx, cy);

            // ----------------------------- 1) ОСТРІВ -----------------------------

            var i0 = P(150, 780);  // лівий край
            var i1 = P(220, 780);
            var i2 = P(280, 770);
            var i3 = P(450, 700);
            var i4 = P(550, 750);  // біля стовбура

            var i5 = P(780, 750);  // правіше стовбура
            var i6 = P(1000, 750);
            var i7 = P(1030, 800);
            var i8 = P(600, 805);

            // кільце зшите: остання вершина замикається назад в i0
            AddSegment(i0, P(200, 760, true), i1);
            AddSegment(i1, P(240, 750, true), i2);
            AddSegment(i2, P(350, 680, true), i3);
            AddSegment(i3, P(500, 720, true), i4);

            AddSegment(i5, P(900, 730, true), i6);
            AddSegment(i6, P(1020, 760, true), i7);
            AddSegment(i7, P(800, 810, true), i8);
            AddSegment(i8, P(400, 810, true), i0);   // повертаємося у ту ж саму i0

            // ----------------------------- 2) СТОВБУР -----------------------------

            // Використовуємо i4 і i5 як базові точки стовбура на острові
            var trunkMidL  = P(680, 500);
            var trunkMidR  = P(700, 600);

            AddSegment(i4,      P(650, 600, true), trunkMidL);
            AddSegment(trunkMidL, P(700, 420, true), center);

            var topRight = P(cx + 15, cy + 10);
            AddSegment(topRight, P(730, 480, true), trunkMidR);
            AddSegment(trunkMidR, P(680, 700, true), i5);

            // ----------------------------- 3) ЛИСТЯ -----------------------------

            // Лист 1 (ліво-верхній)
            var l1_tip1 = P(580, 280);
            var l1_tip2 = P(540, 300);
            var l1_mid1 = P(590, 330);
            var l1_mid2 = P(630, 340);

            AddSegment(center,  P(650, 300, true), l1_tip1);
            AddSegment(l1_tip1, P(560, 275, true), l1_tip2);
            AddSegment(l1_tip2, P(570, 310, true), l1_mid1);
            AddSegment(l1_mid1, P(610, 310, true), l1_mid2);
            AddSegment(l1_mid2, P(660, 350, true), center);

            // Лист 2 (ліво-нижній)
            var l2_a = P(580, 400);
            var l2_b = P(520, 460);
            var l2_c = P(560, 440);
            var l2_d = P(610, 430);

            AddSegment(center, P(640, 380, true), l2_a);
            AddSegment(l2_a,   P(550, 420, true), l2_b);
            AddSegment(l2_b,   P(540, 430, true), l2_c);
            AddSegment(l2_c,   P(580, 410, true), l2_d);
            AddSegment(l2_d,   P(650, 410, true), center);

            // Лист 3 (великий правий)
            var l3_a = P(850, 180);
            var l3_b = P(980, 280);
            var l3_c = P(900, 290);
            var l3_d = P(840, 270);
            var l3_e = P(800, 300);

            AddSegment(center, P(750, 200, true), l3_a);
            AddSegment(l3_a,   P(920, 200, true), l3_b);
            AddSegment(l3_b,   P(940, 250, true), l3_c);
            AddSegment(l3_c,   P(880, 240, true), l3_d);
            AddSegment(l3_d,   P(800, 300, true), l3_e);
            AddSegment(l3_e,   P(800, 300, true), center);

            // Лист 4 (правий нижній)
            var l4_a = P(820, 380);
            var l4_b = P(850, 450);
            var l4_c = P(800, 440);

            AddSegment(center, P(780, 350, true), l4_a);
            AddSegment(l4_a,   P(840, 400, true), l4_b);
            AddSegment(l4_b,   P(820, 420, true), l4_c);
            AddSegment(l4_c,   P(780, 400, true), center);

            // Маленький лист по центру
            var l5_tip = P(720, 250);
            AddSegment(center, P(700, 300, true), l5_tip);
            AddSegment(l5_tip, P(730, 300, true), center);
        }

        // ======================= Побудова кривої =======================

        public void CreateCurvePoints()
        {
            CurvePoints.Clear();

            for (int i = 0; i < Points.Count - 2; i += 3)
            {
                var P0 = Points[i];
                var P1 = Points[i + 1];
                var P2 = Points[i + 2];

                for (double u = 0; u <= 1; u += 0.02)
                {
                    double b0 = (1 - u) * (1 - u);
                    double b1 = 2 * u * (1 - u) * HyperbolaWeight;
                    double b2 = u * u;

                    double denom = b0 + b1 + b2;

                    double x = (b0 * P0.X + b1 * P1.X + b2 * P2.X) / denom;
                    double y = (b0 * P0.Y + b1 * P1.Y + b2 * P2.Y) / denom;

                    CurvePoints.Add(new Point(x, y));
                }

                CurvePoints.Add(new Point(P2.X, P2.Y));
            }
        }
    }
}
