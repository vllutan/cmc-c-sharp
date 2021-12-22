using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;

namespace lab1_V1Data
{
    // ---------------------------------------   V1DataList   -------------------------------------------

    class V1DataList : V1Data
    {
        public List<DataItem> data { get; }

        public V1DataList(string s, DateTime d) : base(s, d)
        {
            data = new List<DataItem>();
        }

        public bool Add(DataItem newItem)
        {
            foreach (DataItem item in data)
            {
                if ((item.x == newItem.x) && (item.y == newItem.y))
                {
                    return false;
                }
            }

            data.Add(newItem);
            return true;
        }

        public int AddDefaults(int nItems, FdblComplex F)
        {
            Random rnd = new Random();
            int addSum = 0;

            for (int i = 0; i < nItems; ++i)
            {
                // квадрат nItem * nItem с центром (0,0)
                double x = rnd.NextDouble() * 2 * nItems - nItems;
                double y = rnd.NextDouble() * 2 * nItems - nItems;
                DataItem di = new DataItem(x, y, F(x, y));

                addSum += Add(di) ? 1 : 0;
            }

            return addSum;
        }

        public override int Count
        {
            get => data.Count;
        }

        public override double AverageValue
        {
            get
            {
                double sum = 0;
                foreach (var item in data) sum += item.c.Magnitude;
                return sum / data.Count;
            }
        }

        public override string ToString()
        {
            return $"Type: V1DataList (List<DataItem>); {base.ToString()}; num of elements: {data.Count}";
        }

        public override string ToLongString(string format)
        {
            string res = ToString() + "\n";
            foreach (var item in data)
            {
                res += item.ToLongString(format) + "\n";
            }

            return res;
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public static bool SaveAsText(string filename, V1DataList v1)
        {
            StreamWriter writer = null;
            try
            {
                using (writer = new StreamWriter(filename))
                {
                    writer.WriteLine(v1.obj);
                    writer.WriteLine(v1.dt);
                    foreach (DataItem di in v1.data)
                    {
                        writer.WriteLine($"{di.x} {di.y} {di.c.Real} {di.c.Imaginary}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }


        public static bool LoadAsText(string filename, ref V1DataList v1)
        {
            //  Вид:
            //  String of info
            //  DateTime
            //  x y c.Real c.Imaginary \n
            //  ...
            StreamReader reader = null;
            try
            {
                if (File.Exists(filename))
                {
                    using (reader = new StreamReader(filename))
                    {
                        string info = reader.ReadLine();
                        DateTime dt = DateTime.Parse(reader.ReadLine());
                        v1 = new V1DataList(info, dt);
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] coords = line.Split(' ');
                            List<double> coords_d = new List<double>();

                            foreach (string coord in coords)
                            {
                                double value;
                                if (!double.TryParse(coord, out value))
                                {
                                    throw new Exception("Bad value");
                                }
                                coords_d.Add(value);
                            }
                            v1.data.Add(new DataItem(coords_d[0], coords_d[1], new Complex(coords_d[2], coords_d[3])));
                        }

                        return true;
                    }
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

    }
}