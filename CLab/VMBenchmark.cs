using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CLab
{
    public class VMBenchmark : INotifyPropertyChanged
    {
        [DllImport("\\..\\..\\..\\..\\x64\\Debug\\Dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Sin(int len, double[] inp_vec, double[] out_vec, int mode);

        [DllImport("\\..\\..\\..\\..\\x64\\Debug\\Dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Cos(int len, double[] inp_vec, double[] out_vec, int mode);

        [DllImport("\\..\\..\\..\\..\\x64\\Debug\\Dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SinCos(int len, double[] inp_vec, double[] out_vec1, double[] out_vec2, int mode);


        public ObservableCollection<VMTime> time;
        public ObservableCollection<VMTime> Time { get { return time; } }
        public ObservableCollection<VMAccuracy> accuracy { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public VMAccuracy selectedAcc;
        public VMAccuracy SelectedAcc
        {
            get { return selectedAcc; }
            set
            {
                selectedAcc = value;
                OnPropertyChanged(nameof(SelectedAcc));
            }
        }
        public VMTime selectedTime;
        public VMTime SelectedTime
        {
            get { return selectedTime; }
            set
            {
                selectedTime = value;
                OnPropertyChanged(nameof(SelectedTime));
            }
        }

        public VMBenchmark()
        {
            time = new ObservableCollection<VMTime>();
            accuracy = new ObservableCollection<VMAccuracy>();
        }
        public bool AddVMTime(VMGrid g) {
            try
            {
                int len = g.arg_vec_len;
                double[] x = new double[len];
                double[] y_ha = new double[len];
                double[] y_la = new double[len];
                double[] y_ep = new double[len];
                if (len != 1)
                {
                    for (int i = 0; i < len; ++i) x[i] = g.segment[0] + g.grid_step * i;
                }
                else
                {
                    x[0] = (g.segment[1] - g.segment[0]) / 2;
                }
                foreach (double i in x) Console.WriteLine(i);

                VMTime t; //= new();
                Console.WriteLine($"{g.function} {g.arg_vec_len} {g.segment}");
                switch (g.function)
                {
                    case VMf.vmdSin:
                        t = new VMTime(g, x, ref y_ha, ref y_la, ref y_ep, Sin);
                        time.Add(t);
                        break;
                    case VMf.vmdCos:
                        t = new(g, x, ref y_ha, ref y_la, ref y_ep, Cos);
                        time.Add(t);
                        break;
                    case VMf.vmdSinCos:
                        t = new(g, x, ref y_ha, ref y_la, ref y_ep, Sin);
                        time.Add(t);
                        t = new(g, x, ref y_ha, ref y_la, ref y_ep, Cos);
                        time.Add(t);
                        break;
                }
                OnPropertyChanged(nameof(min_LA_HA));
                OnPropertyChanged(nameof(min_EP_HA));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }
        public bool AddVMAccuracy(VMGrid g) {
            try
            {
                int len = g.arg_vec_len;
                double[] x = new double[len];
                double[] y_ha = new double[len];
                double[] y_la = new double[len];
                double[] y_ep = new double[len];
                if (len != 1)
                {
                    for (int i = 0; i < len; ++i) x[i] = g.segment[0] + g.grid_step * i;
                }
                else
                {
                    x[0] = (g.segment[0] - g.segment[1]) / 2;
                }

                VMTime t = new();
                VMAccuracy acc = new();
                switch (g.function)
                {
                    case VMf.vmdSin:
                        t = new(g, x, ref y_ha, ref y_la, ref y_ep, Sin);
                        break;
                    case VMf.vmdCos:
                        t = new(g, x, ref y_ha, ref y_la, ref y_ep, Cos);
                        break;
                    case VMf.vmdSinCos:
                        t = new(g, x, ref y_ha, ref y_la, ref y_ep, Sin);
                        acc = new(g, y_ha, y_la, y_ep);
                        accuracy.Add(acc);
                        t = new(g, x, ref y_ha, ref y_la, ref y_ep, Cos);
                        break;
                }
                acc = new(g, y_ha, y_la, y_ep);
                accuracy.Add(acc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        public double min_e_h;
        public double min_EP_HA
        {
            get
            {
                if (Time.Count == 0) return -1; 
                double min = Time[0].coef_EP_HA;
                foreach (VMTime i in Time)
                {
                    if (i.coef_EP_HA < min)
                    {
                        min = i.coef_EP_HA;
                    }
                }
                min_e_h = min;
                return min;
            }
        }

        public double min_LA_HA
        {
            get
            {
                if (Time.Count == 0) return -1;
                double min = Time[0].coef_LA_HA;
                foreach (VMTime i in Time)
                {
                    if (i.coef_LA_HA < min)
                    {
                        min = i.coef_LA_HA;
                    }
                }
                return min;
            }
        }
    }
}