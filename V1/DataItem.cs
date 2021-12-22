using System;
using System.Numerics;

namespace lab1_V1Data
{
    // ----------------------------------------   DataItem   ----------------------------------------------

    public struct DataItem
    {
        public double x { get; set; }
        public double y { get; set; }
        public Complex c { get; set; }

        public DataItem(double x0, double y0, Complex c0)
        {
            x = x0;
            y = y0;
            c = c0;
        }

        public string ToLongString(string format)
        {
            return String.Format("X: {0}, Y: {1}, Value: {2}, Value mod: {3}",
              x.ToString(format), y.ToString(format), c.ToString(format), c.Magnitude.ToString(format));
        }

        public override string ToString()
        {
            return $"X: {x}, Y: {y}, Value: {c}";
        }
    }

    // ----------------------------------------   delegate   ----------------------------------------------

    public delegate Complex FdblComplex(double x, double y);


}