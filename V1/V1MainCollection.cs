using System;
using System.Collections.Generic;
using System.Linq;

namespace lab1_V1Data
{
    // ------------------------------------   V1MainCollection   ----------------------------------------

    class V1MainCollection
    {
        private List<V1Data> gallery;

        public V1MainCollection()
        {
            gallery = new List<V1Data>();
        }

        public int Count
        {
            get => gallery.Count;
        }

        public V1Data this[int index]
        {
            get => gallery[index];
        }

        public bool Contains(string ID)
        {
            foreach (V1Data item in gallery)
            {
                if (item.obj == ID) return true;
            }
            return false;
        }

        public bool Add(V1Data v1Data)
        {
            if (Contains(v1Data.obj)) return false;
            gallery.Add(v1Data);
            return true;
        }

        public string ToLongString(string format)
        {
            string res = "";
            foreach (var item in gallery)
            {
                res += item.ToLongString(format) + "\n\n";
            }
            return res;
        }

        public override string ToString()
        {
            string res = "";
            foreach (var item in gallery)
            {
                res += item.ToString() + "\n";
            }
            return res;
        }

        // -------------------------------------- LINQ --------------------------------------------------

        // среднее значение модуля поля для всех результатов измерений в коллекции V1MainCollection
        public double ColAverage
        {
            get
            {
                //if (Count == 0) return double.NaN;
                try
                {
                    return (from elem in gallery
                            from item in elem
                            select item.c.Magnitude).Average();
                }
                catch (Exception ex)
                {
                    return double.NaN;
                }
            }
        }

        // значение модуля поля максимально отличается от среднего значения модуля поля среди всех
        // результатов измерений
        public DataItem? MaxDifference
        {
            get
            {
                try
                {
                    //if (Count == 0) return null;
                    double avVal = ColAverage;

                    double max_dist =
                      (from elem in gallery
                       from item in elem
                       select Math.Abs(item.c.Magnitude - avVal)).Max();

                    return (from elem in gallery
                            from item in elem
                            where Math.Abs(item.c.Magnitude - avVal) == max_dist
                            select item).First();
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        // Свойство типа IEnumerable<float>, которое перечисляет без повторов все значения
        // координаты x точек, в которых измерено поле и которые встречаются хотя бы в двух
        // разных элементах из списка List<V1Data>
        public IEnumerable<double> GetX
        {
            get
            {
                try
                {
                    if (Count == 0) return null;

                    var keys = from item in gallery
                               from elem in item
                               group elem by new { key1 = elem.x, key2 = elem.y }
                      into XGroups
                               where XGroups.Count() > 1
                               select XGroups.Key.key1;
                    return keys.Distinct();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                    return null;
                }
            }
        }
    }
}