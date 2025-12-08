using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace lab1;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Model _model;
    private View _view;
    private readonly Draw _drawer;
    private double _pixelsInSm;
    private  CoordinateSystem _system;
    public MainWindow()
    {
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        
        _pixelsInSm = 2;
        _system = new  CoordinateSystem(_pixelsInSm);
        InitializeComponent();
        _model = new Model(_pixelsInSm);
        _view = new View(_pixelsInSm);
        _view.Clone();
        
        _drawer = new Draw(Cnv,_pixelsInSm,_view);
        //_drawer.DrawBegin();
        
        _drawer.Drawer();
        FillTextBoxes();

    }
    

    private void Polygon_bt_Click(object sender, RoutedEventArgs e)
    {
        _pixelsInSm = 2;
        _system = new CoordinateSystem(_pixelsInSm);
        _model = new Model(_pixelsInSm);
        _drawer._pixelsInSm = _pixelsInSm;
        FillTextBoxes();
        _view.Clone();
        _drawer.Drawer();
        XxBox.Text = "1";
        XyBox.Text = "0";
        YxBox.Text = "0";
        YyBox.Text = "1";
        
        Check1.IsChecked = false;
        XxPBox.Text = "200";
        XyPBox.Text = "3";
        WxBox.Text = "12";
        
        YxPBox.Text = "0";
        YyPBox.Text = "500";
        WyBox.Text = "2";

        OxPBox.Text = "0";
        OyPBox.Text = "0";
        WoBox.Text = "500";

        ScaleXBox.Text = "1";
        ScaleYBox.Text = "1";

    }
    
    

    private void FillTextBoxes()
    {
        // Захист від null (якщо модель ще не створила лінії)
        if (Model.Lines == null || Model.Lines.Count == 0) return;

        // Helper функція для безпечного отримання
        string GetDist(string name) => 
            Model.Lines.FirstOrDefault(p => p.Name == name)?.Distance.ToString() ?? "0";

        ABBox.Text = GetDist("AB");
        BCBox.Text = GetDist("BC");
        CDBox.Text = GetDist("CD");
        DABox.Text = GetDist("DA");
    
        EFBox.Text = GetDist("EF");
        GHBox.Text = GetDist("GH");
        IJBox.Text = GetDist("IJ");
        KLBox.Text = GetDist("KL");
        SBBox.Text = GetDist("SideBot");
        STBox.Text = GetDist("SideTop");

        CircRBox.Text = Model.rSmall.ToString();
    }

private void ReadFromTextBoxes()
{
    // --- Актуальні лінії (які ми залишили в Model.cs) ---
    
    // Висота (AB)
    // Використовуємо FirstOrDefault, щоб уникнути помилки, якщо лінії немає
    var lineAB = Model.Lines.FirstOrDefault(p => p.Name == "AB");
    if (lineAB != null && !string.IsNullOrEmpty(ABBox.Text))
    {
        lineAB.Distance = double.Parse(ABBox.Text);
    }

    // Ширина (BC)
    var lineBC = Model.Lines.FirstOrDefault(p => p.Name == "BC");
    if (lineBC != null && !string.IsNullOrEmpty(BCBox.Text))
    {
        lineBC.Distance = double.Parse(BCBox.Text);
    }

    // --- Старі лінії (ВИДАЛИТИ АБО ЗАКОМЕНТУВАТИ) ---
    // Вони більше не існують у вашій фігурі, тому цей код ламатиме програму
    
    /* Model.Lines.First(p => p.Name=="HJ").Distance= double.Parse(HJBox.Text);
    Model.Lines.First(p => p.Name=="AV").Distance= double.Parse(AVBox.Text);
    Model.Lines.First(p => p.Name=="CE").Distance= double.Parse(CEBox.Text);
    Model.Lines.First(p => p.Name=="EF").Distance= double.Parse(EFBox.Text);
    Model.Lines.First(p => p.Name=="ZR").Distance= double.Parse(ZRBox.Text);
    Model.Lines.First(p => p.Name=="VL").Distance= double.Parse(VLBox.Text);
    Model.Lines.First(p => p.Name=="FD").Distance= double.Parse(FDBox.Text);
    Model.Lines.First(p => p.Name=="DH").Distance= double.Parse(DHBox.Text);
    Model.Lines.First(p => p.Name=="RP").Distance= double.Parse(RPBox.Text);
    Model.Lines.First(p => p.Name=="ML").Distance= double.Parse(MLBox.Text);
    Model.Lines.First(p => p.Name=="HG").Distance= double.Parse(HGBox.Text);
    Model.Lines.First(p => p.Name=="GZ").Distance= double.Parse(GZBox.Text);
    Model.Lines.First(p => p.Name=="RY").Distance= double.Parse(RYBox.Text);
    Model.Lines.First(p => p.Name=="MN").Distance= double.Parse(MNBox.Text);
    Model.Lines.First(p => p.Name=="YK").Distance= double.Parse(YKBox.Text);
    Model.Lines.First(p => p.Name=="YU").Distance= double.Parse(YUBox.Text);
    Model.Lines.First(p => p.Name=="UO").Distance= double.Parse(UOBox.Text);
    Model.Lines.First(p => p.Name=="OK").Distance= double.Parse(OKBox.Text);
    Model.Lines.First(p => p.Name=="KM").Distance= double.Parse(KMBox.Text);
    Model.Lines.First(p => p.Name=="ZJ").Distance= double.Parse(ZJBox.Text);
    Model.Lines.First(p => p.Name=="JF").Distance= double.Parse(JFBox.Text);
    */
}
    
    
    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        var regex = new Regex("[^0-9]+");
        e.Handled = regex.IsMatch(e.Text);
    }

    private static string IfEmptyThenOne(string s)
    {
        return string.IsNullOrEmpty(s) ? "1" : s;
    }
    
    private void UpdateLine(string name, TextBox box)
    {
        if (string.IsNullOrEmpty(box.Text)) return;
        var line = Model.Lines.FirstOrDefault(p => p.Name == name);
        if (line != null && double.TryParse(box.Text, out double val))
        {
            line.Distance = val;
            StateHandler();
        }
    }
    
    private void ABBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("AB", ABBox);
    private void BCBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("BC", BCBox);
    private void CDBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("CD", CDBox);
    private void DABox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("DA", DABox);

    private void EFBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("EF", EFBox);
    private void GHBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("GH", GHBox);
    private void IJBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("IJ", IJBox);
    private void KLBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("KL", KLBox);
    private void SBBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("SideBot", SBBox);
    private void STBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateLine("SideTop", STBox);

    private void CircRBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (double.TryParse(CircRBox.Text, out double val))
        {
            Model.rSmall = val;
            StateHandler();
        }
    }

    

    private void Move_bt_OnClick(object sender, RoutedEventArgs e)
    {
        Euclidean.Move(double.Parse(MoveBox.Text),_pixelsInSm);
        StateHandler();
    }

    

    private void Rot_bt_OnClick(object sender, RoutedEventArgs e)
    {
        Euclidean.Rotate(new Point(double.Parse(XRBox.Text)*_pixelsInSm+100,700-double.Parse(YRBox.Text)*_pixelsInSm),double.Parse(AngleBox.Text)  ,_pixelsInSm);
        StateHandler();
    }

    private void Aph_bt_OnClick(object sender, RoutedEventArgs e)
    {
        // _system = new CoordinateSystem(_pixelsInSm);  <-- ВИДАЛИТИ ЦЕЙ РЯДОК
        _model = new Model(_pixelsInSm); // Модель можна перестворити, щоб взяти базові розміри
        _view.Clone();
        
        CalculateCircles(); 
        
        Afftransform(); // Тут відбувається магія руху
        _drawer.TransformDraw();
    }

    private void Afftransform()
    {
        var culture = CultureInfo.InvariantCulture;

        var x = new Point(double.Parse(XxBox.Text, culture), double.Parse(XyBox.Text, culture));
        var y = new Point(double.Parse(YxBox.Text, culture), double.Parse(YyBox.Text, culture));
        var o = new Point(double.Parse(OxBox.Text, culture), double.Parse(OyBox.Text, culture));
    
        Affine.Transform(x, y, o);
    }
    
    private void Proect_bt_OnClick(object sender, RoutedEventArgs e)
    {
        // Цей рядок МАЄ БУТИ тут, щоб скидати сітку перед малюванням
        _system = new CoordinateSystem(_pixelsInSm); 
        
        _model = new Model(_pixelsInSm);
        _view.Clone();
        CalculateCircles();
        
        ProectiveTransform(); // Тепер трансформуємо нову чисту сітку і нову модель
        
        if (CheckScale()) Scale();
        _drawer.TransformDraw();
    }

    private void ProectiveTransform()
    {
        // CultureInfo.InvariantCulture змушує програму чекати крапку, а не кому
        var culture = CultureInfo.InvariantCulture;

        var x = new Point(double.Parse(XxPBox.Text, culture), double.Parse(XyPBox.Text, culture));
        var y = new Point(double.Parse(YxPBox.Text, culture), double.Parse(YyPBox.Text, culture));
        var o = new Point(double.Parse(OxPBox.Text, culture), double.Parse(OyPBox.Text, culture));
    
        Proective.Transform(
            x, 
            y, 
            o, 
            double.Parse(WxBox.Text, culture), 
            double.Parse(WyBox.Text, culture), 
            double.Parse(WoBox.Text, culture)
        );
    }

    private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        if (Check1.IsChecked == true){proect_bt.IsEnabled = true;}
        else
        {
            proect_bt.IsEnabled = false;
        }
    }

    private bool CheckAffine()
    {
        if (XxBox.Text != "1" || XyBox.Text != "0" || YxBox.Text != "0" || YyBox.Text != "1")
        {
            return true;
        }

        return false;
    }

    private bool CheckScale()
    {
        if (ScaleXBox.Text != "1" || ScaleYBox.Text != "1" )
        {
            return true;
        }

        return false;
    }

    private void CalculateCircles()
    {
        _drawer.CalculateCircle(View.rSmall, 0,360,View.N,View.P);
        if (View.Circle != null && View.N != null && View.P != null)
        {
            _drawer.CalculateCircle(View.rSmall, 0, 360, View.N, View.P);
        }
    }


    private void Symm_bt_OnClick(object sender, RoutedEventArgs e)
    {
        Symmetry.Transform(new Point(double.Parse(SymXBox.Text) * _pixelsInSm + 100,700 - double.Parse(SymYBox.Text) * _pixelsInSm));
        StateHandler();

    }

    private void Scale_bt_OnClick(object sender, RoutedEventArgs e)
    {
        Scale();
        _drawer.TransformDraw();
    }

    private void Scale()
    {
        var x = new Point(double.Parse(ScaleXBox.Text), 0);
        var y = new Point(0, double.Parse(ScaleYBox.Text));
        var o = new Point(0, 0);
        Affine.Transform(x,y,o);
    }

    private void StateHandler()
    {
        _view.Clone();
        _system = new CoordinateSystem(_pixelsInSm);
        CalculateCircles();
        if (CheckAffine())
        {
            Afftransform();
            _drawer.TransformDraw(); 
        }else if (Check1.IsChecked == true)
        {
            ProectiveTransform();
            _drawer.TransformDraw(); 
        }
        if (CheckScale())
        {
            Scale();
            _drawer.TransformDraw(); 
        }

        if (CheckScale() || CheckAffine() || Check1.IsChecked == true) return;
        View.Circle = new Point[720]; 
        _drawer.Drawer();
    }
}