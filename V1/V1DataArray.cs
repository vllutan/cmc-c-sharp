using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;

namespace lab1_V1Data
{
    // ---------------------------------------   V1DataArray   -----------------------------------------------------

    class V1DataArray : V1Data
    {
        public int nodeX { get; private set; }
        public double stepX { get; private set; }
        public int nodeY { get; private set; }
        public double stepY { get; private set; }

        public Complex[,] valueArr { get; private set; }

        public V1DataArray(string s, DateTime d) : base(s, d)
        {
            valueArr = new Complex[0, 0];
        }

        public V1DataArray(string s, DateTime d, int nX, int nY, double sX, double sY,
          FdblComplex F) : base(s, d)
        {
            nodeX = nX;
            nodeY = nY;
            stepX = sX;
            stepY = sY;

            valueArr = new Complex[nodeX, nodeY];
            for (int i = 0; i < nodeX; ++i)
            {
                for (int j = 0; j < nodeY; ++j)
                {
                    valueArr[i, j] = F(i * stepX, j * stepY);
                }
            }
        }

        public override int Count
        {
            get => nodeX * nodeY;
        }

        public override double AverageValue
        {
            get
            {
                double sum = 0;
                for (var i = 0; i < nodeX; i++)
                    for (var j = 0; j < nodeY; j++)
                        sum += valueArr[i, j].Magnitude;
                return sum / Count;
            }
        }

        public override string ToString()
        {
            return $"Type: V1DataArray (Complex[,]); {base.ToString()}" + "\n" +
              $"node: X - {nodeX}; Y - {nodeY}; step: X - {stepX}; Y - {stepY}";
        }

        public override string ToLongString(string format)
        {
            string res = ToString() + "\n";
            for (var i = 0; i < nodeX; i++)
                for (var j = 0; j < nodeY; j++)
                {
                    DataItem di = new DataItem(i * stepX, j * stepY, valueArr[i, j]);
                    res += di.ToLongString(format) + "\n";
                }

            return res;
        }

        public static explicit operator V1DataList(V1DataArray arr)
        {
            V1DataList list = new V1DataList(arr.obj + " -> list", arr.dt);
            for (var i = 0; i < arr.nodeX; i++)
            {
                for (var j = 0; j < arr.nodeY; j++)
                {
                    DataItem di = new DataItem(i * arr.stepX, j * arr.stepY, arr.valueArr[i, j]);
                    list.Add(di);
                }
            }

            return list;
        }
        //implicit

        public override IEnumerator<DataItem> GetEnumerator()
        {
            for (var i = 0; i < nodeX; i++)
            {
                for (var j = 0; j < nodeY; j++)
                {
                    yield return new DataItem(i * stepX, j * stepY, valueArr[i, j]);
                }
            }
        }

        public static bool SaveBinary(string filename, V1DataArray v1)
        {
            BinaryWriter writer = null;
            try
            {
                using (writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    writer.Write(v1.obj);
                    writer.Write(v1.dt.ToString());
                    writer.Write(v1.nodeX);
                    writer.Write(v1.nodeY);
                    writer.Write(v1.stepX);
                    writer.Write(v1.stepY);
                    for (var i = 0; i < v1.nodeX; i++)
                    {
                        for (var j = 0; j < v1.nodeY; j++)
                        {
                            writer.Write(i);
                            writer.Write(j);
                            writer.Write(v1.valueArr[i, j].Real);
                            writer.Write(v1.valueArr[i, j].Imaginary);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exeption: {ex.Message}");
                return false;
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

        public static bool LoadBinary(string filename, ref V1DataArray v1)
        {
            //  Вид:
            //  Info string
            //  DateTime
            //  nodeX \n nodeY \n stepX \n stepY
            //  x \n y \n c.Real \n c.Imaginary 
            //  ...

            BinaryReader reader = null;
            try
            {
                if (File.Exists(filename))
                {
                    using (reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                    {
                        string info = reader.ReadString();
                        DateTime datetime = DateTime.Parse(reader.ReadString());
                        int nX = reader.ReadInt32();
                        int nY = reader.ReadInt32();
                        double sX = reader.ReadDouble();
                        double sY = reader.ReadDouble();
                        v1 = new V1DataArray(info, datetime, nX, nY, sX, sY, (x, y) => new Complex(x, y));
                        for (var i = 0; i < v1.nodeX; i++)
                        {
                            for (var j = 0; j < v1.nodeY; j++)
                            {
                                int x = reader.ReadInt32();
                                int y = reader.ReadInt32();
                                double cR = reader.ReadDouble();
                                double cI = reader.ReadDouble();
                                v1.valueArr[x, y] = new Complex(cR, cI);
                            }
                        }
                    }

                    return true;
                }
                else throw new Exception("No such file\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        // ---------------------------------------- Lab3 ------------------------------

        public Complex? FieldAt(int jx, int jy)
        {
            return valueArr[jx, jy];
        }

        public bool Max_Field_Re(int jy, ref double min, ref double max)
        {
            if ((jy < 0) || (jy > nodeY)) return false;
            IEnumerable<int> jx = Enumerable.Range(0, nodeX).Select(x => x);
            IEnumerable<double> vec =
              from item in jx
              select valueArr[item, jy].Real;
            /*foreach (double item in vec) { Console.WriteLine(item); }*/
            min = vec.Min();
            max = vec.Max();
            return true;
        }

        public bool Max_Field_Im(int jy, ref double min, ref double max)
        {
            if ((jy < 0) || (jy > nodeY)) return false;
            IEnumerable<int> jx = Enumerable.Range(0, nodeX).Select(x => x);
            IEnumerable<double> vec =
              from item in jx
              select valueArr[item, jy].Imaginary;
            /*foreach (double item in vec) { Console.WriteLine(item); }*/
            min = vec.Min();
            max = vec.Max();
            return true;
        }

        [DllImport("Dll.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CalcMKL(int nx, double[] x, int ny, double[] y, int ns, double[] coeff, double[] result, ref int error);

        //[DllImport("Dll.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern void Hello();

        public V1DataArray ToSmallerGrid(int ns)
        {
            try
            {
                double sx = stepX * (nodeX - 1) / (ns - 1),
                       sy = stepY;
                int error = 0;
                double[] x = new double[2] { 0, (nodeX - 1) * stepX };
                double[] y = new double[2 * nodeX * nodeY];
                double[] coeff = new double[2* nodeY * 4 * (nodeX - 1)];
                double[] site = new double[2] { 0, (ns - 1) * sx };
                double[] result = new double[ns * 2 * nodeY];

                for (int i = 0; i < nodeY; ++i)
                {
                    for (int j = 0; j < nodeX; ++j)
                    {
                        y[2 * i * nodeX + j] = valueArr[j, i].Real;
                    }
                    for (int j = 0; j < nodeX; ++j)
                    {
                        y[(2 * i + 1) * nodeX + j] = valueArr[j, i].Imaginary;
                    }
                }

                CalcMKL(nodeX, x, 2 * nodeY, y, ns, coeff, result, ref error);

                if (error != 0)
                {
                    Console.WriteLine("Error from MKL: " + error);
                    return null;
                }

                V1DataArray smallArr = new V1DataArray("scaled " + this.obj, this.dt);
                smallArr.nodeX = ns;
                smallArr.nodeY = nodeY;
                smallArr.stepX = sx;
                smallArr.stepY = sy;
                smallArr.valueArr = new Complex[smallArr.nodeX, smallArr.nodeY];
                
                for (int i = 0; i < nodeY; ++i)
                {
                    for (int j = 0; j < ns; ++j)
                    {
                        smallArr.valueArr[j, i] = new Complex(result[2 * ns * i + j], result[2 * ns * i + ns + j]);
                    }
                }
                return smallArr;
            }
            catch (Exception ex)
            {
                Console.WriteLine("  Exception thrown: " + ex);
                return null;
            }
        }
    }
}