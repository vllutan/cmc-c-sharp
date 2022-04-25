namespace CLab
{
    public struct VMAccuracy
    {
        public VMGrid grid { get; set; }

        public double max_abs_diff { get; set; } // |HA - EP|
        public double max_abs_arg { get; set; }  // argument

        public double[] max_diff_arg_info { get; set; }  //  HA, LA, EP

        public VMAccuracy()
        {
            grid = new VMGrid();
            max_abs_diff = 0;
            max_abs_arg = 0;
            max_diff_arg_info = new double[3] { 0,0,0 };
        }
        public VMAccuracy(VMGrid gr, double[] y_ha, double[] y_la, double[] y_ep) : this()
        {
            try
            {
                grid = gr;
                int ind = 0;
                double max = 0;
                double cur = 0;
                for (int i = 0; i < grid.arg_vec_len; ++i)
                {
                    cur = Math.Abs(y_ha[i] - y_ep[i]);
                    if (cur > max)
                    {
                        ind = i;
                        max = cur;
                    }
                }
                max_abs_diff = max;
                max_abs_arg = grid.segment[0] + grid.grid_step * ind;
                max_diff_arg_info = new double[] { y_ha[ind], y_la[ind], y_ep[ind] };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public override string ToString()
        {
            return $"Grid: {grid.ToString()}\n" +
                $"argument: {max_abs_arg}\n|HA-EP|: {max_abs_diff} \n ";
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
                return $"HA: {max_diff_arg_info[0]}\nLA: {max_diff_arg_info[1]}\nEP: {max_diff_arg_info[2]}";
            }
        }
    }
}