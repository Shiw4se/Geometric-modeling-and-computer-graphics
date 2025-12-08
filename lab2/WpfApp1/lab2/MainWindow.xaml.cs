using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace lab2;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Drawer _drawer;
    private double _pixelsInSm;
    private bool isAnimated;
    private CoordinateSystem _system; 
    
    // Прапорець, щоб події не спрацьовували до повної ініціалізації
    private bool _isInitialized = false;

    public MainWindow()
    {
        // Встановлюємо культуру, щоб завжди використовувати крапку як роздільник (0.5)
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        InitializeComponent();
        
        _pixelsInSm = 2.0;
        
        // 1. Створюємо систему координат
        _system = new CoordinateSystem(_pixelsInSm);
        
        // 2. Ініціалізуємо Drawer
        _drawer = new Drawer(Cnv, _pixelsInSm);
        
        // 3. Створюємо початкову модель (A=100, K=3)
        var model = new Model(_pixelsInSm, 100, 3);
        
        // 4. Оновлюємо UI значеннями з коду
        ABox.Text = "100";
        KBox.Text = "3";
        isAnimated = false;

        // 5. Позначаємо, що ініціалізація завершена
        _isInitialized = true;

        // 6. Перше малювання
        RedrawAll();
    }

    // --- ГОЛОВНИЙ МЕТОД МАЛЮВАННЯ ---
    private void RedrawAll()
    {
        if (!_isInitialized) return; 

        // 1. Очищаємо старі лінії перед новим малюванням
        if (Model.Tangents != null) Model.Tangents.Clear();
        if (Model.Normals != null) Model.Normals.Clear();

        // 2. Якщо галочка стоїть — рахуємо нові
        if (AsymptCheckBox.IsChecked == true)
        {
            int counter = 0;
            int step = 30; // Малюємо лінію для кожної 30-ї точки (можна змінити)

            // Model.Curves — це словник, де ключ = кут (phi), значення = Точка
            foreach (var item in Model.Curves)
            {
                if (counter % step == 0)
                {
                    double phi = item.Key;
                    Point p = item.Value;

                    // Рахуємо і додаємо в списки Model.Tangents / Model.Normals
                    var tangent = new Tangent(p, phi);
                    tangent.CalculateTangent();

                    var normal = new Normal(p, phi);
                    normal.CalculateNormal();
                }
                counter++;
            }
        }

        // 3. Малюємо все на екрані
        _drawer.DrawModel(); 

        // 4. Оновлюємо цифри
        SBox.Text = Calculator.CalculateSquare().ToString("F2");
        LBox.Text = Calculator.CalculateLineLength().ToString("F2");
    }

    // --- ОБРОБНИКИ ЗМІНИ ПАРАМЕТРІВ КРИВОЇ ---

    private void ABox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        // БЛОКУЄМО виконання, якщо вікно ще не завантажилось
        if (!_isInitialized) return;

        if (double.TryParse(ABox.Text, out double val))
        {
            Model.A = val;
            new Model(_pixelsInSm, Model.A, Model.K); // Перестворюємо модель
            RedrawAll();
        }
    }

    private void KBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isInitialized) return;

        if (double.TryParse(KBox.Text, out double val))
        {
            Model.K = val;
            new Model(_pixelsInSm, Model.A, Model.K);
            RedrawAll();
        }
    }

    // --- АНІМАЦІЯ ---

    private void ButtonAnimation_OnClick(object sender, RoutedEventArgs e)
    {
        if (!isAnimated)
        {
            DrawAnimatedModel();
        }
    }

    private async Task DrawAnimatedModel()
    {
        isAnimated = true;
        
        // Анімуємо параметр K від поточного значення до +6
        double startK = Model.K;
        double endK = startK + 6; 
        double step = 0.05;

        for (double k = startK; k <= endK; k += step)
        {
            if (!isAnimated) break;

            // Оновлюємо K
            Model.K = k;
            new Model(_pixelsInSm, Model.A, Model.K); // Перерахунок точок
            
            // Оновлюємо UI (текстбокс), тимчасово вимикаючи події, щоб не було зациклення
            _isInitialized = false; 
            KBox.Text = k.ToString("F2");
            _isInitialized = true;
            
            RedrawAll();
            
            await Task.Delay(50); // Затримка 50мс між кадрами
        }
        
        isAnimated = false;
    }

    private void ButtonPauseAnimation_OnClick(object sender, RoutedEventArgs e)
    {
        isAnimated = false;
        _drawer.PauseAnimation();
    }

    // --- ТРАНСФОРМАЦІЇ (MOVE / ROTATE) ---

    private void ButtonMove_OnClick(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(MXox.Text, out double dx) && double.TryParse(MYox.Text, out double dy))
        {
            Euclid.Move(dx, dy, _pixelsInSm);
            RedrawAll();
        }
    }

    private void Rot_bt_OnClick(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(XRBox.Text, out double cx) && 
            double.TryParse(YRBox.Text, out double cy) &&
            double.TryParse(AngleBox.Text, out double angle))
        {
            // Центр повороту
            var center = new Point(cx, cy); 
            
            Euclid.Rotate(center, angle, _pixelsInSm);
            RedrawAll();
        }
    }

    // --- ДОДАТКОВІ КНОПКИ ---

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        // Тут можна викликати анімацію малювання точки по кривій
        // _drawer.DrawCurves(Directions.Bottom);
    }

    private void ButtonTop_OnClick(object sender, RoutedEventArgs e)
    {
       
        
        
        Model.K = 5; // Наприклад, 5 пелюсток
        new Model(_pixelsInSm, Model.A, Model.K);
        KBox.Text = "5";
        RedrawAll();
        
    }
    
    private void ButtonPause_OnClick(object sender, RoutedEventArgs e)
    {
        // Скидання вигляду (Reset)
        _isInitialized = false; // Пауза подій
        ABox.Text = "100";
        KBox.Text = "3";
        _isInitialized = true;

        _system = new CoordinateSystem(_pixelsInSm); // Скидаємо сітку в початковий стан
        var model = new Model(_pixelsInSm, 100, 3);
        RedrawAll();
    }

    private void AsymptCheckBox_OnChecked(object sender, RoutedEventArgs e)
    {
        RedrawAll();
    }

    private void Polygon_bt_Click(object sender, RoutedEventArgs e)
    {
        // Кнопка Reset Shape робить те саме, що й Pause (скидає)
        ButtonPause_OnClick(sender, e);
    }
}