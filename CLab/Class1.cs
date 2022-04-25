using System.ComponentModel;

namespace CLab
{
    [Serializable]
    public enum VMf
    {
        vmdSin, 
        vmdCos,
        vmdSinCos
    }

    public class VMGrid : INotifyPropertyChanged
    {
        public VMf function { get; set; }
        public int arg_vec_len { get; set; }
        public double[] segment { get; set; }
        public double grid_step { get; }

        public VMGrid()
        {
            function = VMf.vmdSin;
            arg_vec_len = 100;
            segment = new double[2]; 
            segment[0] = 0;
            segment[1] = 1;
            if (arg_vec_len > 1) grid_step = (segment[1] - segment[0]) / (arg_vec_len - 1);
            else grid_step = 0;
        }

        public VMGrid(VMf func, int len, double[] seg)
        {
            function = func;
            arg_vec_len = len;
            segment = new double[2];
            segment[0] = seg[0];
            segment[1] = seg[1];
            if (arg_vec_len > 1) grid_step = (segment[1] - segment[0]) / (arg_vec_len - 1);
            else grid_step = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return $"{function}, [{segment[0]},{segment[1]}], {arg_vec_len} nodes";
        }

        public string ToLongString()
        {
            return $"function {function} on the segment [{segment[0]}, {segment[1]}] with {arg_vec_len} arguments and grid step {grid_step}.";
        }
    }

    
}