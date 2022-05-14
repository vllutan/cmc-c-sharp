using ClassLibrary2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;


namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public class ChartData
    {
        public SeriesCollection sc { get; set; }
        public Func<double, string> Formatter { get; set; }
        public ChartData()
        {
            sc = new();
            Formatter = value => value.ToString("F3");
        }

        public void MakePlot(double[] grid, double[] data, int mode, string title)
        {
            try
            {
                ChartValues<ObservablePoint> values = new ChartValues<ObservablePoint>();
                for (int i = 0; i < data.Length; i++)
                {
                    values.Add(new(grid[i], data[i]));
                }
                if (mode == 1) sc.Add(new LineSeries { Title = title, Values = values, PointGeometry = null });
                else sc.Add(new ScatterSeries { Title = title, Values = values, PointGeometry = DefaultGeometries.Circle });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void Clear()
        {
            sc.Clear();
        }
    }

    public class ViewData : INotifyPropertyChanged
    {
        public SplinesData splData { get; set; } = new();
        public ChartData cd { get; set; } = new();

        public ViewData() { }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CanExecuteChanged;

        protected void OnPropertyChanged(string property_name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
            }
        }
        
        public void Clear()
        {
            cd.Clear();
            splData.mdata.exists = false;
            splData.integral = 0;
            splData.mdata.Str = null;
            OnPropertyChanged(nameof(splData));
            OnPropertyChanged(nameof(splData.mdata));

        }
        
    }

    public static class Cmd
    {
        public static readonly RoutedUICommand MeasuredData = new
            (
                "MeasuredData",
                "MeasuredData",
                typeof(Cmd),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D1, ModifierKeys.Control)
                }
            );

        public static readonly RoutedUICommand Splines = new
        (
            "Splines",
            "Splines",
            typeof(Cmd),
            new InputGestureCollection()
            {
                    new KeyGesture(Key.D2, ModifierKeys.Control)
            }
        );
    }
    public partial class MainWindow : Window
    {
        public ViewData vd { get; set; } = new();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
            function_choice.SelectedItem = button_lin;
        }

        private void MD_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = vd.splData.mdata.IfCorrect();
        }

        private void MD_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SPf spf = new SPf();
            if (function_choice.SelectedItem == button_lin) spf = SPf.linear;
            else if (function_choice.SelectedItem == button_cub) spf = SPf.cubic;
            else if (function_choice.SelectedItem == button_rand) spf = SPf.random;
            else if (function_choice.SelectedItem == null) { MessageBox.Show("Choose function"); return; }
            

            vd.splData.mdata.function = spf;
            vd.splData.mdata.SetData();
            vd.splData.spl_param.segment[0] = vd.splData.mdata.segment0;
            vd.splData.spl_param.segment[1] = vd.splData.mdata.segment1;
            vd.cd.MakePlot(vd.splData.mdata.nodes, vd.splData.mdata.data, 2, "Points");

        }

        private void Spl_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = vd.splData.spl_param.IfCorrect() && vd.splData.mdata.exists && vd.splData.mdata.IfCorrect();
        }

        private void Spl_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            vd.splData.GetSplineAndIntegral();
            vd.cd.MakePlot(vd.splData.grid, vd.splData.spline_data, 1, "Points");
        }

        private void clear_click(object sender, RoutedEventArgs e)
        {
            vd.Clear();
        }

        
    }
}
