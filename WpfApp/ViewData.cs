using CLab;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public class ViewData : INotifyPropertyChanged
    {
        public VMBenchmark vmb = new VMBenchmark();

        public VMBenchmark Vmb
        {
            get
            {
                return vmb;
            }
            set
            {
                vmb = value;
                OnPropertyChanged(nameof(Vmb));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string property_name)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
            }
        }
        public ViewData()
        {
            vmb = new VMBenchmark();
        }

        public void AddVMTime(VMGrid vmg)
        {
            Change = vmb.AddVMTime(vmg);
        }

        public void AddVMAccuracy(VMGrid vmg)
        {
            Change = vmb.AddVMAccuracy(vmg);
        }

        public bool Save(string filename)
        {
            FileStream file = null;
            try
            {
                using (file = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ViewData));
                    xmlSerializer.Serialize(file, this);
                    Change = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception during file saving: {ex.Message}");
                return false;
            }
            finally
            {
                if (file != null) file.Close();
            }
        }

        public static bool Load(string filename, ref ViewData vd)
        {
            FileStream file = null;
            try
            {
                using (file = new FileStream(filename, FileMode.Open))
                {
                    vd.vmb.accuracy.Clear();
                    vd.vmb.time.Clear();
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ViewData));
                    vd = xmlSerializer.Deserialize(file) as ViewData;
                    vd.Change = false;
                }
                return true;
            }
            catch (Exception ex)
            {
                vd = new ViewData();
                MessageBox.Show($"Exception during file loading: {ex.Message}");
                return false;
            }
            finally
            {
                if (file != null) file.Close();
            }
        }

        bool change = false;
        public bool Change 
        { 
            get { return change; } 
            set
            {
                change = value;
                OnPropertyChanged(nameof(ifChanged));
            }
        }
        public string ifChanged
        {
            get
            { 
                if (change) return "Collection was changed";
                else return "Collection wasn't changed";
            }
        }
    }
}
