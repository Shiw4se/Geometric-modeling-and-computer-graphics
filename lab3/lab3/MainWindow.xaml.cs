using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace lab3;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Draw _drawer;
    private double _pixelsInSm;
    private bool _isInitialized = false;
    
    // Для перетягування (хоча основна логіка тепер у Draw)
    private Ellipse? selectedPoint = null; 
    private Point offset;
    
    private Model _model;
    
    public MainWindow()
    {
        // Встановлюємо культуру для коректної обробки крапки в числах (1.5 замість 1,5)
        System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

        InitializeComponent(); 
        
        _pixelsInSm = 2.0;
        
        // 1. Ініціалізуємо систему координат (статичні поля)
        var system = new Coordinate(_pixelsInSm);
        
        // 2. Створюємо модель (точки пальми)
        _model = new Model(_pixelsInSm);
        
        // 3. Ініціалізуємо малювальника
        _drawer = new Draw(Cnv, _pixelsInSm, _model);
        
        // Налаштування початкового стану з UI
        _drawer.ShowPoints = ShowSkeletonCheck.IsChecked == true;
        _drawer.EnableEditing = EditModeCheck.IsChecked == true;

        // Події миші для Canvas (для додаткової логіки, якщо потрібно, або делегування в Draw)
        // Основна логіка перетягування вже реалізована всередині класу Draw (через події на Ellipse),
        // але ці події корисні для кліків по порожньому місцю або правої кнопки.
        Cnv.MouseRightButtonDown += MouseRightButtonClicked;
        
        // 4. Перше малювання
        _drawer.DrawModel();
        
        _isInitialized = true;
    }

    // --- ОБРОБКА ПОДІЙ МИШІ ---

    private void MouseRightButtonClicked(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is Ellipse pointEllipse)
        {
            var pointData = pointEllipse.Tag as Point;
            if (pointData == null)
                return;

            // Реперні точки (зелені) не чіпаємо – тільки вузлові (червоні)
            if (!pointData.isControl)
            {
                // Тоглимо: вузлова ↔ точка зламу
                pointData.IsBreak = !pointData.IsBreak;

                _model.CreateCurvePoints();
                _drawer.DrawModel();
            }
        }
    }


    // --- НАЛАШТУВАННЯ КРИВОЇ ---

    private void WeightBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isInitialized) return;

        if (double.TryParse(WeightBox.Text, out double w))
        {
            // Оновлюємо вагу для гіперболи
            Model.HyperbolaWeight = w;
            _model.CreateCurvePoints();
            _drawer.DrawModel();
        }
    }

    // --- ВІЗУАЛІЗАЦІЯ (Чекбокси) ---

    private void ShowSkeletonCheck_OnChecked(object sender, RoutedEventArgs e)
    {
        if (_drawer == null) return;
        _drawer.ShowPoints = ShowSkeletonCheck.IsChecked == true;
        _drawer.DrawModel();
    }

    private void EditModeCheck_OnChecked(object sender, RoutedEventArgs e)
    {
        if (_drawer == null) return;
        _drawer.EnableEditing = EditModeCheck.IsChecked == true;
        // Перемальовуємо, щоб оновити підписки на події (в Draw.CreatePoint)
        _drawer.DrawModel(); 
    }

    // --- АНІМАЦІЯ ---

    private async void ButtonAnimate_OnClick(object sender, RoutedEventArgs e)
    {
        // Запускаємо морфінг
        await _drawer.DrawAnimation();
    }

    private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
    {
        // Скидаємо модель до початкового стану
        _model = new Model(_pixelsInSm);
        
        // Оновлюємо посилання в Drawer (або створюємо новий Drawer)
        // Оскільки Drawer тримає посилання на стару модель, треба створити новий.
        _drawer = new Draw(Cnv, _pixelsInSm, _model);
        
        // Відновлюємо налаштування
        _drawer.ShowPoints = ShowSkeletonCheck.IsChecked == true;
        _drawer.EnableEditing = EditModeCheck.IsChecked == true;
        
        // Скидаємо UI
        MXox.Text = "10"; MYox.Text = "10";
        XRBox.Text = "400"; YRBox.Text = "400"; AngleBox.Text = "30";
        WeightBox.Text = "2.0";
        Model.HyperbolaWeight = 2.0;

        _drawer.DrawModel();
    }
    
    // --- ТРАНСФОРМАЦІЇ ---

    private void ButtonMove_OnClick(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(MXox.Text, out double dx) && double.TryParse(MYox.Text, out double dy))
        {
            Euclidian.Move(dx, dy, _model);
            // CreateCurvePoints викликається всередині Euclidian.Move
            _drawer.DrawModel();
        }
    }
    
    private void Rot_bt_OnClick(object sender, RoutedEventArgs e)
    {
        if (double.TryParse(XRBox.Text, out double cx) && 
            double.TryParse(YRBox.Text, out double cy) &&
            double.TryParse(AngleBox.Text, out double angle))
        {
            var center = new Point(cx, cy);
            Euclidian.Rotate(center, angle, _model);
            // CreateCurvePoints викликається всередині Euclidian.Rotate
            _drawer.DrawModel();
        }
    }

    private void Symm_bt_OnClick(object sender, RoutedEventArgs e)
    {
        // Реалізація дзеркального відображення (симетрії)
        // Для простоти можна реалізувати симетрію відносно вертикальної осі, що проходить через центр
        if (double.TryParse(SymXBox.Text, out double centerX))
        {
            foreach (var p in _model.Points)
            {
                // x' = center + (center - x) = 2*center - x
                p.X = 2 * centerX - p.X;
            }
            _model.CreateCurvePoints();
            _drawer.DrawModel();
        }
    }

    private void Scale_bt_OnClick(object sender, RoutedEventArgs e)
    {
        // Реалізація масштабування відносно центру (XRBox, YRBox) або початку координат
        if (double.TryParse(ScaleBox.Text, out double scale) && 
            double.TryParse(XRBox.Text, out double cx) && 
            double.TryParse(YRBox.Text, out double cy))
        {
            foreach (var p in _model.Points)
            {
                p.X = cx + (p.X - cx) * scale;
                p.Y = cy + (p.Y - cy) * scale;
            }
            _model.CreateCurvePoints();
            _drawer.DrawModel();
        }
    }

    // Кнопка Pause тепер працює як Reset View (з вашого XAML)
    private void ButtonPause_OnClick(object sender, RoutedEventArgs e)
    {
        _drawer.isAnimated = false; // Зупинити анімацію, якщо вона йде
    }
}