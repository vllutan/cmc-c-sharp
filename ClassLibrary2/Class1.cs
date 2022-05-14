using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ClassLibrary2
{

    public enum SPf
    {
        linear,
        cubic,
        random
    }

    public class Methods
    {
        public static void Linear(double[] x, double[] y)
        {
            for(int i=0; i<x.Length; ++i)
            {
                y[i] = x[i];
            }
        }

        public static void Cubic(double[] x, double[] y)
        {
            for (int i = 0; i < x.Length; ++i)
            {
                y[i] = x[i] * x[i] * x[i] - 5 * x[i] + 11;
            }
        }

        public static void Random(double[] x, double[] y)
        {
            Random rand = new Random();
            for (int i = 0; i < x.Length; ++i)
            {
                y[i] = rand.NextDouble() * 5;
            }
        }

        public static void Coordinates(double limits0, double limits1, double[] y)
        {
            double dif = limits1 - limits0;
            Random rand = new Random();
            y[0] = limits0;
            for (int i = 1; i < y.Length-1; ++i)
            {
                y[i] = rand.NextDouble() * dif + limits0;
            }
            y[y.Length-1] = limits1;
            Array.Sort(y);
        }
    }

    public class MeasuredData : IDataErrorInfo, INotifyPropertyChanged
    {
        public int len { get; set; }
        public double segment0 { get; set; }
        public double segment1 { get; set; }
        public SPf function { get; set; }
        public double int_limits0 { get; set; }
        public double int_limits1 { get; set; }
        public bool exists { get; set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property_name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
            }
        }

        public double[] nodes { get; set; }
        public double[] data { get; set; }

        public bool IfCorrect()
        {
            return (len >= 3) && (segment0 < segment1) && (int_limits0 >= segment0) && (int_limits1 <= segment1) && (int_limits0 < int_limits1);
        }

        public string Error => throw new NotImplementedException();

        public string this[string propertyName]
        {
            get
            {
                string error = string.Empty;

                switch (propertyName)
                {
                    case nameof(len):
                        if (len < 3)
                            error = "The number of nodes must be greater than 2.";
                        break;

                    case nameof(segment0):
                        if (segment0 >= segment1)
                            error = "Left limit of irregular grid must be lesser than the right limit.";
                        break;

                    case nameof(segment1):
                        if (segment0 >= segment1)
                            error = "Left limit of irregular grid must be lesser than the right limit.";
                        break;

                    case nameof(int_limits0):
                        if (int_limits0 < segment0 || int_limits1 > segment1 || int_limits0 >= int_limits1)
                            error = "Integral limits must be different.";
                        break;

                    case nameof(int_limits1):
                        if (int_limits0 < segment0 || int_limits1 > segment1 || int_limits0 >= int_limits1)
                            error = "Integral limits must be different.";
                        break;

                    default:
                        error = "";
                        break;
                }

                return error;
            }
        } 

        public MeasuredData()
        {
            len = 3;
            segment0 = 0;
            segment1 = 1;
            function = SPf.linear;
            nodes = new double[3];
            data = new double[3];
            int_limits0 = 0;
            int_limits1 = 1;
            Methods.Linear(nodes, data);
            OnPropertyChanged(nameof(Info));
        }

        public MeasuredData(int l, double seg0, double seg1, SPf f)
        {
            len = l;
            segment0 = seg0;
            segment1 = seg1;
            function = f;
            nodes = new double[l];
            Methods.Coordinates(segment0, segment1, nodes);
            data = new double[l];
            switch (f)
            {
                case SPf.linear:
                    Methods.Linear(nodes, data);
                    break;
                case SPf.cubic:
                    Methods.Cubic(nodes, data);
                    break;
                case SPf.random:
                    Methods.Random(nodes, data);
                    break;
                default:
                    throw new Exception("function not defined");
            }
            OnPropertyChanged(nameof(Info));
            exists = true;
        }

        public bool SetData()
        {
            try
            {
                nodes = new double[len];
                Methods.Coordinates(segment0, segment1, nodes);
                data = new double[len];
                switch (function)
                {
                    case SPf.linear:
                        Methods.Linear(nodes, data);
                        break;
                    case SPf.cubic:
                        Methods.Cubic(nodes, data);
                        break;
                    case SPf.random:
                        Methods.Random(nodes, data);
                        break;
                    default:
                        throw new Exception("function not defined");
                }
                OnPropertyChanged(nameof(Str));
                exists = true;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        public ObservableCollection<string>? _str;
        public ObservableCollection<string>? Str
        {
            get
            {
                _str = new();
                for (int i = 0; i < len; i++) _str.Add($"x[{i + 1}]: {nodes[i]:f8}\t\ty[{i + 1}]: {data[i]:f8}");
                OnPropertyChanged(nameof(_str));
                return _str;
            }
            set
            {
                _str = value;
                OnPropertyChanged(nameof(_str));
            }
        }
        public string Info
        {
            get
            {
                string ret = string.Empty;
                ret += $"{len} nodes:\n";
                for (int i = 0; i < nodes.Length; ++i)
                    ret += $" {nodes[i]}  -> {data[i]}\n";
                return ret;
            }
        }

    }

    public class SplineParameters : IDataErrorInfo
    {
        public int len { get; set; }
        public double[] segment { get; set; }
        public double[] derivative { get; set; }

        public bool IfCorrect()
        {
            return (len >= 3);
        }

        public string Error => throw new NotImplementedException();

        public string this[string propertyName]
        {
            get
            {
                string error = string.Empty;

                switch (propertyName)
                {
                    case nameof(len):
                        if (len < 3)
                            error = "The number of nodes must be greater than 2.";
                        break;

                    default:
                        error = "";
                        break;
                }

                return error;
            }
        }

        public SplineParameters()
        {
            len = 3;
            segment = new double[2] { 0,1 };
            derivative = new double[2] { -1,1 };
        }

        public SplineParameters(int l, double[] seg, double[] der, double[] lim)
        {
            len = l;
            segment = seg;
            derivative = der;
        }
    }

    public class SplinesData : INotifyPropertyChanged
    {
        public MeasuredData mdata { get; set; }
        public SplineParameters spl_param { get; set; }

        public double[] grid { get; set; }
        public double[] spline_data { get; set; }
        public double integral { get; set; }

        public SplinesData()
        {
            mdata = new MeasuredData();
            spl_param = new SplineParameters();
            integral = 0;
            spline_data = new double[spl_param.len];
            grid = new double[spl_param.len];
        }
        public SplinesData(MeasuredData md, SplineParameters sp)
        {
            mdata = md;
            spl_param = sp;
            integral = 0;
            spline_data = new double[spl_param.len];
            grid = new double[spl_param.len];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property_name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property_name));
            }
        }

        [DllImport("\\..\\..\\..\\..\\x64\\Debug\\Dll2.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CalcMKL(int nx, double[] x, int ny, double[] y, double[] lim, int ns, double[] site, double[] coeff, 
                  double[] result, ref int error, double[] int_left, double[] int_right, double[] int_res);
            
        public int GetSplineAndIntegral()
        {
            try
            {
                int error = 0;
                double[] coeff = new double[4 * (mdata.len - 1)];
                double[] left = new double[1] { mdata.int_limits0 };
                double[] right = new double[1] { mdata.int_limits1 };
                double[] intg = new double[1];
                spline_data = new double[spl_param.len];
                grid = new double[spl_param.len];
                for (int i = 0; i < spl_param.len; i++)
                    grid[i] = spl_param.segment[0] + i * (spl_param.segment[1] - spl_param.segment[0]) / (spl_param.len-1);

                int ret = CalcMKL(mdata.len, mdata.nodes, 1, mdata.data, spl_param.derivative, spl_param.len, spl_param.segment, coeff, 
                    spline_data, ref error, left, right, intg);

                foreach(var i in spline_data) Console.WriteLine(i.ToString());

                if (error != 0)
                {
                    Console.WriteLine("Error from MKL: " + error);
                    throw new Exception(error.ToString());
                    return ret+10;
                }

                integral = intg[0];
                OnPropertyChanged(nameof(Info));
                return ret+20;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -3;
            }
        }

        public string Info
        {
            get
            {
                string ret = string.Empty;
                ret += $"derivatives:\n\tleft: {spl_param.derivative[0]},  right: {spl_param.derivative[1]}\n";
                ret += $"integral: {integral}\n";
                return ret;
            }
        }
    }

    //class Program
    //{
    //    static void Main()
    //    {
    //        MeasuredData ms = new(5, 0,2, SPf.cubic);
    //        SplineParameters spl = new SplineParameters(100, new double[2] { 0, 2 }, new double[2] { -1,1}, new double[2] { 0, 2 });
    //        SplinesData sd = new(ms, spl);
    //        int err = sd.GetSplineAndIntegral();
    //        Console.WriteLine(err.ToString());
    //    }
    //}


}