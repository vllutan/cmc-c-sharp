using CLab;
using System;
using System.Collections.Generic;
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

namespace WpfApp
{

    public partial class MainWindow : Window
    {
        ViewData vd = new ViewData();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = vd;
        }

        private void MenuFile_New(object sender, RoutedEventArgs e)
        {
            if (vd.Change == true)
            {
                MessageBoxResult res = MessageBox.Show("Data will be lost. Save Data?", "", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Yes)
                {
                    MenuFile_Save(sender, e);
                }
                else if (res == MessageBoxResult.Cancel) return;
            }

            vd.vmb.time.Clear();
            vd.vmb.accuracy.Clear();
            vd = new ViewData();
            DataContext = vd;
        }

        private void MenuFile_Open(object sender, RoutedEventArgs e)
        {
            if (vd.Change == true)
            {
                MessageBoxResult res = MessageBox.Show("Data will be lost. Save Data?", "", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Yes)
                {
                    MenuFile_Save(sender, e);
                }
                else if (res == MessageBoxResult.Cancel) return;
            }

            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            if(dialog.ShowDialog() == true)
            {
                if (!ViewData.Load(dialog.FileName, ref vd))
                {
                    MessageBox.Show($"File not loaded correctly");
                    vd = new ViewData();
                }
                DataContext = vd;
            }
        }

        private void MenuFile_Save(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                if (!vd.Save(dialog.FileName))
                {
                    MessageBox.Show($"File not saved correctly");
                }
            }
        }

        private void MenuEdit_AddTime_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VMf vmf = new VMf();
                if (function_choice.SelectedItem == button_Sin) vmf = VMf.vmdSin;
                else if (function_choice.SelectedItem == button_Cos) vmf = VMf.vmdCos;
                else if (function_choice.SelectedItem == null) { MessageBox.Show("Choose function"); return; }

                int len = int.Parse(tbox_len.Text);
                double[] seg = new double[2];
                seg[0] = double.Parse(tbox_left.Text);
                seg[1] = double.Parse(tbox_right.Text);
                if (seg[0] > seg[1]) { MessageBox.Show("Left limit greater than the right limit"); return; }

                VMGrid vmg = new VMGrid(vmf, len, seg);
                vd.Change = vd.vmb.AddVMTime(vmg);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Incorrect input: " + ex.Message);
            }
        }

        private void MenuEdit_AddAcc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VMf vmf = new VMf();
                if (function_choice.SelectedItem == button_Sin) vmf = VMf.vmdSin;
                else if (function_choice.SelectedItem == button_Cos) vmf = VMf.vmdCos;
                else if (function_choice.SelectedItem == null) { MessageBox.Show("Choose function"); return; }

                int len = int.Parse(tbox_len.Text);
                double[] seg = new double[2];
                seg[0] = double.Parse(tbox_left.Text);
                seg[1] = double.Parse(tbox_right.Text);
                if(seg[0] > seg[1]) { MessageBox.Show("Left limit greater than the right limit"); return; }

                VMGrid vmg = new VMGrid(vmf, len, seg);
                vd.Change = vd.vmb.AddVMAccuracy(vmg);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Incorrect input: "+ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(MessageBox.Show("Close window?", "", MessageBoxButton.YesNoCancel) == MessageBoxResult.No) e.Cancel = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (vd.Change == true)
            {
                if (MessageBox.Show("Data will be lost. Save Data?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        if (!vd.Save(dialog.FileName))
                        {
                            MessageBox.Show($"File not saved correctly");
                        }
                    }
                }
            }
        }
    }
}
