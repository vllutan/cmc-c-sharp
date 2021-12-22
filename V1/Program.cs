using System;
using System.Numerics;

namespace lab1_V1Data
{

    
    // -----------------------------   Main and Tests   ---------------------------------

    class Program
    {

        // ---------------------------------- tests ----------------------------------

        static void Lab1Test()
        {
            string format = "f3";

            V1DataArray arr1 = new V1DataArray("arr1", DateTime.Now, 2, 2, 0, 0, Methods.Method1);
            Console.WriteLine(arr1.ToLongString(format));

            V1DataList list1 = (V1DataList)arr1;
            Console.WriteLine(list1.ToLongString(format));

            Console.WriteLine($"arr: Count {arr1.Count}, AverageValue {arr1.AverageValue}" + "\n" +
              $"list: Count {list1.Count}, AverageValue {list1.AverageValue}\n");

            V1DataArray arr2 = new V1DataArray("arr2", DateTime.Now, 2, 2, 0.25, 0.25, Methods.Method1);
            V1DataList list2 = new V1DataList("list2", DateTime.Now);
            list2.AddDefaults(6, Methods.Method1);

            V1MainCollection coll = new V1MainCollection();
            coll.Add(arr1);
            coll.Add(arr2);
            coll.Add(list1);
            coll.Add(list2);

            Console.WriteLine(coll.ToLongString("f3"));
            Console.WriteLine(coll.ToString());

            for (int i = 0; i < coll.Count; ++i)
            {
                Console.WriteLine($"Count: {coll[i].Count}, AvVal: {coll[i].AverageValue}");
            }
        }

        static void TestLINQ1()
        {
            V1DataList list1 = new V1DataList("list1", DateTime.Now);
            list1.AddDefaults(2, Methods.Method1);

            V1DataList list2 = new V1DataList("list2", DateTime.Now);
            list2.AddDefaults(2, Methods.Method1);

            V1DataArray arr1 = new V1DataArray("arr1", DateTime.Now, 2, 2, 0.25, 0.25, Methods.Method1);

            V1DataArray arr2 = new V1DataArray("arr2", DateTime.Now, 2, 2, 0.5, 0.5, Methods.Method1);

            V1DataList list3 = (V1DataList)arr2;

            V1MainCollection coll = new V1MainCollection();
            coll.Add(list1);
            coll.Add(list2);
            coll.Add(arr1);
            coll.Add(arr2);
            coll.Add(list3);

            Console.WriteLine(coll.ToLongString("f3"));
            Console.WriteLine("\nAverage in the whole collection: " + coll.ColAverage);
            Console.WriteLine("\nItem with max distance from average: " + coll.MaxDifference);
            if (coll.GetX is not null)
            {
                Console.WriteLine("\nX values that are in several elements of collection: ");
                foreach (double i in coll.GetX) Console.WriteLine(i);
            }

            //Console.WriteLine(coll.GetX);
        }

        static void TestLINQ2()
        {
            V1MainCollection coll = new V1MainCollection();
            Console.WriteLine(coll.ToLongString("f3"));
            Console.WriteLine(coll.ColAverage);
            Console.WriteLine(coll.MaxDifference is null);
            Console.WriteLine(coll.GetX is null);
        }

        static void TestLINQ3()
        {
            V1DataList list = new V1DataList("list", DateTime.Now);
            list.AddDefaults(0, Methods.Method1);

            //V1DataList list2 = new V1DataList("list2", DateTime.Now);


            V1DataArray arr1 = new V1DataArray("arr1", DateTime.Now, 0, 0, 0.25, 0.25, Methods.Method1);
            V1DataList list1 = (V1DataList)arr1;

            V1DataArray arr2 = new V1DataArray("arr2", DateTime.Now, 2, 2, 0, 0, Methods.Method1);
            V1DataList list2 = (V1DataList)arr2;

            V1MainCollection coll = new V1MainCollection();
            coll.Add(list);
            coll.Add(arr1);
            coll.Add(arr2);
            coll.Add(list1);
            coll.Add(list2);


            Console.WriteLine(coll.ToLongString("f3"));
            Console.WriteLine("\nAverage in the whole collection: " + coll.ColAverage);
            Console.WriteLine("\nItem with max distance from average: " + coll.MaxDifference);
            if (coll.GetX is not null)
            {
                Console.WriteLine("\nX values that are in several elements of collection: ");
                foreach (double i in coll.GetX) Console.WriteLine(i);
            }
        }

        static void TestInOutList1()
        {
            V1DataList list = new V1DataList("list", DateTime.Now);
            list.AddDefaults(2, Methods.Method1);
            Console.WriteLine(list.ToLongString("f7") + "\n");

            Console.WriteLine($"Saved - {V1DataList.SaveAsText("test1", list)}");

            V1DataList list2 = new V1DataList("returned list", DateTime.Now);
            Console.WriteLine($"Loaded - {V1DataList.LoadAsText("test1", ref list2)}");
            Console.WriteLine(list2.ToLongString("f7"));
        }

        static void TestInOutList2()
        {
            V1DataList list = new V1DataList("list", DateTime.Now);
            list.AddDefaults(0, Methods.Method1);
            Console.WriteLine(list.ToLongString("f7") + "\n");

            Console.WriteLine($"Saved - {V1DataList.SaveAsText("test1", list)}");

            V1DataList list2 = new V1DataList("returned list", DateTime.Now);
            Console.WriteLine($"Loaded - {V1DataList.LoadAsText("test1", ref list2)}");
            Console.WriteLine(list2.ToLongString("f7"));
        }

        static void TestInOutArray1()
        {
            V1DataArray arr = new V1DataArray("arr", DateTime.Now, 2, 2, 0.25, 0.25, Methods.Method1);
            Console.WriteLine(arr.ToLongString("f7") + "\n");

            Console.WriteLine($"Saved - {V1DataArray.SaveBinary("test2", arr)}");

            V1DataArray arr2 = new V1DataArray("returned arr", DateTime.Now);
            Console.WriteLine($"Loaded - {V1DataArray.LoadBinary("test2", ref arr2)}");
            Console.WriteLine(arr2.ToLongString("f7"));
        }

        static void TestInOutArray2()
        {
            V1DataArray arr = new V1DataArray("arr", DateTime.Now, 0, 0, 0.25, 0.25, Methods.Method1);
            Console.WriteLine(arr.ToLongString("f7") + "\n");

            Console.WriteLine($"Saved - {V1DataArray.SaveBinary("test2", arr)}");

            V1DataArray arr2 = new V1DataArray("returned arr", DateTime.Now);
            Console.WriteLine($"Loaded - {V1DataArray.LoadBinary("test2", ref arr2)}");
            Console.WriteLine(arr2.ToLongString("f7"));
        }


        /* --------------------------------------------------------- test for Lab3 -----------------------------------*/


        static void TestLab3SupportFunc()
        {
            Console.WriteLine("\n______Support functions' test_____\n");

            V1DataArray arr = new V1DataArray("arr", DateTime.Now, 3, 3, 0.5, 0.5, Methods.Method2);
            Console.WriteLine(arr.ToLongString("f3"));

            double min = 0, max = 0;
            int jy = 1;

            Console.WriteLine(arr.Max_Field_Re(jy, ref min, ref max));
            Console.WriteLine("min " + min + ",   max " + max);

            Console.WriteLine(arr.Max_Field_Im(jy, ref min, ref max));
            Console.WriteLine("min " + min + ",   max " + max);

            Console.WriteLine(arr.Max_Field_Im(4, ref min, ref max));     // jy > nodeX=3 
        }

        static void ArrayMinMaxOutput(ref V1DataArray arr)
            /*отдельная функция для вывода минимумов и максимумов у массива*/
        {
            double min = 0, max = 0;

            Console.WriteLine($"\n    {arr.obj}:  ");
            Console.WriteLine("  Re:  ");
            for (int i = 0; i < arr.nodeY; ++i)
            {
                arr.Max_Field_Re(i, ref min, ref max);
                Console.WriteLine($"y={i} :  min - {min},  max - {max}");
            }

            Console.WriteLine("  Im:  ");
            for (int i = 0; i < arr.nodeY; ++i)
            {
                arr.Max_Field_Im(i, ref min, ref max);
                Console.WriteLine($"y={i} :  min - {min},  max - {max}");
            }
            Console.WriteLine("\n");
        }

        static void TestLab3MainFunc1()
        {
            Console.WriteLine("\n\n______test 1_____\n");

            V1DataArray arr = new V1DataArray("arr", DateTime.Now, 3, 3, 1, 1, Methods.Method2);
            Console.WriteLine(arr.ToLongString("f3"));

            V1DataArray arr_s = arr.ToSmallerGrid(9);
            Console.WriteLine(arr_s.ToLongString("f3"));

            ArrayMinMaxOutput(ref arr);
            ArrayMinMaxOutput(ref arr_s);
        }

        static void TestLab3MainFunc2()
        {
            Console.WriteLine("\n\n______test 2_____\n");

            V1DataArray arr2 = new V1DataArray("arr", DateTime.Now, 2, 2, 1, 1, Methods.Method2);
            Console.WriteLine(arr2.ToLongString("f3"));

            V1DataArray arr2_s = arr2.ToSmallerGrid(4);
            Console.WriteLine(arr2_s.ToLongString("f3"));

            ArrayMinMaxOutput(ref arr2);
            ArrayMinMaxOutput(ref arr2_s);
        }



        static void Main(string[] args)
        {
            TestLab3SupportFunc(); // 3 other function 
            TestLab3MainFunc1();   // 3x3 -> 9x3 /*большой тест*/
            TestLab3MainFunc2();   //2x2 -> 4x2 /*тест поменьше*/
        }
    }
}