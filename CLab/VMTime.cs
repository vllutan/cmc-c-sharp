using System.Diagnostics;

namespace CLab
{
    public delegate bool func(int len, double[] inp_vec, double[] out_vec, int mode);

    public struct VMTime
    {
        public VMGrid grid { get; set; }

        public TimeSpan[] time { get; set; } // VML_HA, VML_LA, VML_EP
        public double coef_LA_HA { get; set; }
        public double coef_EP_HA { get; set; }

        public VMTime()
        {
            grid = new VMGrid();
            time = new TimeSpan[3];
            coef_LA_HA = 0;
            coef_EP_HA = 0;
        }

        public VMTime(VMGrid gr, double[] x, ref double[] y1, ref double[] y2, ref double[] y3, func f) : this()
        {
            try
            {
                grid = gr;
                int avl = gr.arg_vec_len;
                time = new TimeSpan[3];
                Stopwatch stopWatch = new Stopwatch();

                stopWatch.Start();
                f(avl, x, y1, 1); // HA
                stopWatch.Stop();
                time[0] = stopWatch.Elapsed;
                stopWatch.Reset();

                stopWatch.Start();
                f(gr.arg_vec_len, x, y2, 2); // LA
                stopWatch.Stop();
                time[1] = stopWatch.Elapsed;
                stopWatch.Reset();

                stopWatch.Start();
                f(gr.arg_vec_len, x, y3, 3); // EP
                stopWatch.Stop();
                time[2] = stopWatch.Elapsed;
                stopWatch.Reset();

                coef_LA_HA = time[1] / time[0];
                coef_EP_HA = time[2] / time[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception raised: " + ex.ToString());
            }
        }

        public override string ToString()
        {
            return $"Grid: {grid.ToString()}\n" +
                $"Time: \n VML_HA - {time[0]}\n VML_LA - {time[1]}\n VML_EP - {time[2]} \n";
        }

        public string description
        {
            get
            {
                return this.ToString();
            }
        }

        public string Info
        {
            get
            {
                if (grid == null) return "";
                return $"LA to HA - {coef_LA_HA:f6} \n EP to HA - {coef_EP_HA:f6} \n";
                
            }
        }
    }
}