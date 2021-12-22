using System.Numerics;

namespace lab1_V1Data
{
    // -----------------------------   Methods   ---------------------------------

    static class Methods
    {

        public static Complex Method1(double x, double y)
        {
            Complex c = new Complex(x, y);
            return c * c - 1;
        }

        public static Complex Method2(double x, double y)
        {
            Complex c = new Complex(x, y);
            return c * c * c - 3 * c * c - c + 5;       // c^3 - 3c^2 - c + 5
        }

    }
}