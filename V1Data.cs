using System;
using System.Collections.Generic;
using System.Numerics;
using System.Collections;
using System.Linq;
using System.IO;

namespace lab1_V1Data
{
  // ----------------------------------------   DataItem   ----------------------------------------------
  
  public struct DataItem {
    public double x { get; set; }
    public double y { get; set; }
    public Complex c { get; set; }

    public DataItem(double x0, double y0, Complex c0) {
      x = x0;
      y = y0;
      c = c0;
    }

    public string ToLongString(string format) {
      return String.Format("X: {0}, Y: {1}, Value: {2}, Value mod: {3}",
        x.ToString(format), y.ToString(format), c.ToString(format), c.Magnitude.ToString(format));
    }
    
    public override string ToString() {
      return $"X: {x}, Y: {y}, Value: {c}";
    }
  }

  // ----------------------------------------   delegate   ----------------------------------------------
  
  public delegate Complex FdblComplex(double x, double y);

  // --------------------------------------------   V1Data   --------------------------------------------
  
  abstract class V1Data : IEnumerable<DataItem> {
    public string obj { get; protected set; }

    public DateTime dt { get; protected set; }

    public V1Data(string s, DateTime d) {
      obj = s;
      dt = d;
    }

    public abstract int Count { get; }
    public abstract double AverageValue { get; }
    public abstract string ToLongString(string format);
    public override string ToString() {
      return $"Object {obj}; DateTime {dt}";
    }
    
    public abstract IEnumerator<DataItem> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
  }

  // ---------------------------------------   V1DataList   -------------------------------------------

  class V1DataList : V1Data {
    public List<DataItem> data { get; }

    public V1DataList(string s, DateTime d) : base(s, d) {
      data = new List<DataItem>();
    }

    public bool Add(DataItem newItem) {
      foreach (DataItem item in data) {
        if ((item.x == newItem.x) && (item.y == newItem.y)) {
          return false;
        }
      }

      data.Add(newItem);
      return true;
    }

    public int AddDefaults(int nItems, FdblComplex F) {
      Random rnd = new Random();
      int addSum = 0;

      for (int i = 0; i < nItems; ++i) {
        // квадрат nItem * nItem с центром (0,0)
        double x = rnd.NextDouble() * 2 * nItems - nItems;
        double y = rnd.NextDouble() * 2 * nItems - nItems;
        DataItem di = new DataItem(x, y, F(x, y));

        addSum += Add(di) ? 1 : 0;
      }

      return addSum;
    }

    public override int Count {
      get => data.Count;
    }

    public override double AverageValue {
      get {
        double sum = 0;
        foreach (var item in data) sum += item.c.Magnitude;
        return sum / data.Count;
      }
    }

    public override string ToString() {
      return $"Type: V1DataList (List<DataItem>); {base.ToString()}; num of elements: {data.Count}";
    }

    public override string ToLongString(string format) {
      string res = ToString() + "\n";
      foreach (var item in data) {
        res += item.ToLongString(format) + "\n";
      }

      return res;
    }

    public override IEnumerator<DataItem> GetEnumerator() {
      return data.GetEnumerator();
    }

    // -------------------------------------------- save and load -----------------------------------------

    public static bool SaveAsText(string filename, V1DataList v1) {
      StreamWriter writer = null;
      try {
        using (writer = new StreamWriter(filename)) {
          writer.WriteLine(v1.obj);
          writer.WriteLine(v1.dt);
          foreach (DataItem di in v1.data) {
            writer.WriteLine($"{di.x} {di.y} {di.c.Real} {di.c.Imaginary}");
          }
        }

        return true;
      }
      catch (Exception ex) {
        Console.WriteLine($"Exception: {ex.Message}");
        return false;
      }
      finally {
        if (writer != null) writer.Close();
      }
    }


    public static bool LoadAsText(string filename, ref V1DataList v1) {
      //  Вид:
      //  String of info
      //  DateTime
      //  x y c.Real c.Imaginary \n
      //  ...
      StreamReader reader = null;
      try {
        if (File.Exists(filename)) {
          using (reader = new StreamReader(filename)) {
            string info = reader.ReadLine();
            DateTime dt = DateTime.Parse(reader.ReadLine());
            v1 = new V1DataList(info, dt);
            string line;
            while ((line = reader.ReadLine()) != null) {
              string[] coords = line.Split(' ');
              List<double> coords_d = new List<double>();

              foreach (string coord in coords) {
                double value;
                if (!double.TryParse(coord, out value)) {
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
      catch (Exception ex) {
        Console.WriteLine($"Exception: {ex.Message}");
        return false;
      }
      finally {
        if (reader != null) reader.Close();
      }
    }

  }

  // ---------------------------------------   V1DataArray   -----------------------------------------------------
  
  class V1DataArray : V1Data {
    public int nodeX { get; private set; }
    public double stepX { get; private set; }
    public int nodeY { get; private set; }
    public double stepY { get; private set; }

    public Complex[,] valueArr { get; private set; }

    public V1DataArray(string s, DateTime d) : base(s, d) {
      valueArr = new Complex[0,0];
    }
    
    public V1DataArray(string s, DateTime d, int nX, int nY, double sX, double sY, 
      FdblComplex F) : base(s,d) {
      nodeX = nX;
      nodeY = nY;
      stepX = sX;
      stepY = sY;
      
      valueArr = new Complex[nodeX, nodeY];
      for (int i = 0; i < nodeX; ++i) {
        for (int j = 0; j < nodeY; ++j) {
          valueArr[i, j] = F(i * stepX, j * stepY);
        }
      }
    }

    public override int Count {
      get => nodeX * nodeY; 
    }

    public override double AverageValue {
      get {
        double sum = 0;
        for (var i = 0; i < nodeX; i++) 
          for (var j = 0; j < nodeY; j++)
            sum += valueArr[i,j].Magnitude;
        return sum / Count;
      }
    }

    public override string ToString() {
      return $"Type: V1DataArray (Complex[,]); {base.ToString()}" + "\n" +
        $"node: X - {nodeX}; Y - {nodeY}; step: X - {stepX}; Y - {stepY}";
    }

    public override string ToLongString(string format) {
      string res = ToString() + "\n";
      for (var i = 0; i < nodeX; i++)
        for (var j = 0; j < nodeY; j++) {
          DataItem di = new DataItem(i * stepX, j * stepY, valueArr[i, j]);
          res += di.ToLongString(format) + "\n";
        }

      return res;
    }

    public static explicit operator V1DataList(V1DataArray arr) {
      V1DataList list = new V1DataList(arr.obj + " -> list", arr.dt);
      for (var i = 0; i < arr.nodeX; i++) {
        for (var j = 0; j < arr.nodeY; j++) {
          DataItem di = new DataItem(i * arr.stepX, j * arr.stepY, arr.valueArr[i, j]);
          list.Add(di);
        }
      }

      return list;
    }
    //implicit
    
    public override IEnumerator<DataItem> GetEnumerator(){
      for (var i = 0; i < nodeX; i++) {
        for (var j = 0; j < nodeY; j++) {
          yield return new DataItem(i * stepX, j * stepY, valueArr[i, j]);
        }
      }
    }
    
    // ---------------------------------------- save and load ----------------------------------------------

    public static bool SaveBinary(string filename, V1DataArray v1) {
      BinaryWriter writer = null;
      try {
        using (writer = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate))) {
          writer.Write(v1.obj);
          writer.Write(v1.dt.ToString());
          writer.Write(v1.nodeX);
          writer.Write(v1.nodeY);
          writer.Write(v1.stepX);
          writer.Write(v1.stepY);
          for (var i = 0; i < v1.nodeX; i++) {
            for (var j = 0; j < v1.nodeY; j++) {
              writer.Write(i);
              writer.Write(j);
              writer.Write(v1.valueArr[i,j].Real);
              writer.Write(v1.valueArr[i,j].Imaginary);
            }
          }
        }
        return true;
      }
      catch(Exception ex) {
        Console.WriteLine($"Exeption: {ex.Message}");
        return false;
      }
      finally {
        if (writer != null) writer.Close();
      }
    }

    public static bool LoadBinary(string filename, ref V1DataArray v1) {
      //  Вид:
      //  Info string
      //  DateTime
      //  nodeX \n nodeY \n stepX \n stepY
      //  x \n y \n c.Real \n c.Imaginary 
      //  ...

      BinaryReader reader = null;
      try {
        if (File.Exists(filename)) {
          using (reader = new BinaryReader(File.Open(filename, FileMode.Open))) {
            string info = reader.ReadString();
            DateTime datetime = DateTime.Parse(reader.ReadString());
            int nX = reader.ReadInt32();
            int nY = reader.ReadInt32();
            double sX = reader.ReadDouble();
            double sY = reader.ReadDouble();
            v1 = new V1DataArray(info, datetime, nX, nY, sX, sY, (x, y) => new Complex(x, y));
            for (var i = 0; i < v1.nodeX; i++) {
              for (var j = 0; j < v1.nodeY; j++) {
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
      catch(Exception ex) {
        Console.WriteLine($"Exception: {ex.Message}");
        return false;
      }
      finally {
        if(reader != null) reader.Close();
      }
    }

  }

  // ------------------------------------   V1MainCollection   ----------------------------------------
  
  class V1MainCollection {
    private List<V1Data> gallery;

    public V1MainCollection() {
      gallery = new List<V1Data>();
    }

    public int Count {
      get => gallery.Count;
    }

    public V1Data this[int index] {
      get => gallery[index]; 
    }

    public bool Contains(string ID) {
      foreach (V1Data item in gallery) {
        if (item.obj == ID) return true;
      }
      return false;
    }

    public bool Add(V1Data v1Data) {
      if (Contains(v1Data.obj)) return false;
      gallery.Add(v1Data);
      return true;
    }

    public string ToLongString(string format) {
      string res = "";
      foreach (var item in gallery) {
        res += item.ToLongString(format) + "\n\n";
      }
      return res;
    }

    public override string ToString() {
      string res = "";
      foreach (var item in gallery) {
        res += item.ToString() + "\n";
      }
      return res;
    }
    
    // -------------------------------------- LINQ --------------------------------------------------

    // среднее значение модуля поля для всех результатов измерений в коллекции V1MainCollection
    public double ColAverage {
      get {
        //if (Count == 0) return double.NaN;
        try {
          return (from elem in gallery
            from item in elem
            select item.c.Magnitude).Average();
        }
        catch (Exception ex) {
          return double.NaN;
        }
      }
    }

    // значение модуля поля максимально отличается от среднего значения модуля поля среди всех
    // результатов измерений
    public DataItem? MaxDifference {
      get {
        try {
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
        catch (Exception ex) {
          return null;
        }
      }
    }

    // Свойство типа IEnumerable<float>, которое перечисляет без повторов все значения
    // координаты x точек, в которых измерено поле и которые встречаются хотя бы в двух
    // разных элементах из списка List<V1Data>
    public IEnumerable<double> GetX {
      get {
        try {
          if (Count == 0) return null;
          
          var keys = from item in gallery
            from elem in item
            group elem by new {key1 = elem.x, key2 = elem.y}
            into XGroups
            where XGroups.Count() > 1
            select XGroups.Key.key1;
          return keys.Distinct();
        }
        catch (Exception ex) {
          Console.WriteLine($"Exception: {ex.Message}");
          return null;
        }
      }
    }
  }
  
  // -----------------------------   Methods   ---------------------------------
  
  static class Methods {
    
    public static Complex Method1(double x, double y) {
      Complex c = new Complex(x, y);
      return c*c - 1;
    }
    
  }
  
  
  // -----------------------------   Main and Tests   ---------------------------------
  
  class Program {

    // ---------------------------------- tests ----------------------------------
    
    static void Lab1Test() {
      string format = "f3";
      
      V1DataArray arr1 = new V1DataArray("arr1", DateTime.Now, 2, 2, 0, 0, Methods.Method1);
      Console.WriteLine(arr1.ToLongString(format));

      V1DataList list1 = (V1DataList) arr1;
      Console.WriteLine(list1.ToLongString(format));
      
      Console.WriteLine($"arr: Count {arr1.Count}, AverageValue {arr1.AverageValue}" + "\n" + 
        $"list: Count {list1.Count}, AverageValue {list1.AverageValue}\n" );

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

      for (int i = 0; i < coll.Count; ++i) {
        Console.WriteLine($"Count: {coll[i].Count}, AvVal: {coll[i].AverageValue}");
      }
    }

    static void TestLINQ1() {
      V1DataList list1 = new V1DataList("list1", DateTime.Now);
      list1.AddDefaults(2, Methods.Method1);
      
      V1DataList list2 = new V1DataList("list2", DateTime.Now);
      list2.AddDefaults(2, Methods.Method1);
      
      V1DataArray arr1 = new V1DataArray("arr1", DateTime.Now, 2, 2, 0.25, 0.25, Methods.Method1);
      
      V1DataArray arr2 = new V1DataArray("arr2", DateTime.Now, 2, 2, 0.5, 0.5, Methods.Method1);
      
      V1DataList list3 = (V1DataList) arr2;

      V1MainCollection coll = new V1MainCollection();
      coll.Add(list1);
      coll.Add(list2);
      coll.Add(arr1);
      coll.Add(arr2);
      coll.Add(list3);
      
      Console.WriteLine(coll.ToLongString("f3"));
      Console.WriteLine("\nAverage in the whole collection: " + coll.ColAverage);
      Console.WriteLine("\nItem with max distance from average: " + coll.MaxDifference);
      if(coll.GetX is not null) {
        Console.WriteLine("\nX values that are in several elements of collection: ");
        foreach (double i in coll.GetX) Console.WriteLine(i);
      }

      //Console.WriteLine(coll.GetX);
    }

    static void TestLINQ2() {
      V1MainCollection coll = new V1MainCollection();
      Console.WriteLine(coll.ToLongString("f3"));
      Console.WriteLine(coll.ColAverage);
      Console.WriteLine(coll.MaxDifference is null);
      Console.WriteLine(coll.GetX is null);
    }

    static void TestLINQ3() {
      V1DataList list = new V1DataList("list", DateTime.Now);
      list.AddDefaults(0, Methods.Method1);
      
      //V1DataList list2 = new V1DataList("list2", DateTime.Now);
      
      
      V1DataArray arr1 = new V1DataArray("arr1", DateTime.Now, 0, 0, 0.25, 0.25, Methods.Method1);
      V1DataList list1 = (V1DataList) arr1;
      
      V1DataArray arr2 = new V1DataArray("arr2", DateTime.Now, 2, 2, 0, 0, Methods.Method1);
      V1DataList list2 = (V1DataList) arr2;

      V1MainCollection coll = new V1MainCollection();
      coll.Add(list);
      coll.Add(arr1);
      coll.Add(arr2);
      coll.Add(list1);
      coll.Add(list2);
      
      
      Console.WriteLine(coll.ToLongString("f3"));
      Console.WriteLine("\nAverage in the whole collection: " + coll.ColAverage);
      Console.WriteLine("\nItem with max distance from average: " + coll.MaxDifference);
      if(coll.GetX is not null) {
        Console.WriteLine("\nX values that are in several elements of collection: ");
        foreach (double i in coll.GetX) Console.WriteLine(i);
      }
    }

    static void TestInOutList1() {
      V1DataList list = new V1DataList("list", DateTime.Now);
      list.AddDefaults(2, Methods.Method1);
      Console.WriteLine(list.ToLongString("f7") + "\n");
      
      Console.WriteLine($"Saved - {V1DataList.SaveAsText("test1", list)}");
      
      V1DataList list2 = new V1DataList("returned list", DateTime.Now);
      Console.WriteLine($"Loaded - {V1DataList.LoadAsText("test1", ref list2)}");
      Console.WriteLine(list2.ToLongString("f7"));
    }
    
    static void TestInOutList2() {
      V1DataList list = new V1DataList("list", DateTime.Now);
      list.AddDefaults(0, Methods.Method1);
      Console.WriteLine(list.ToLongString("f7") + "\n");
      
      Console.WriteLine($"Saved - {V1DataList.SaveAsText("test1", list)}");
      
      V1DataList list2 = new V1DataList("returned list", DateTime.Now);
      Console.WriteLine($"Loaded - {V1DataList.LoadAsText("test1", ref list2)}");
      Console.WriteLine(list2.ToLongString("f7"));
    }

    static void TestInOutArray1() {
      V1DataArray arr = new V1DataArray("arr", DateTime.Now, 2, 2, 0.25, 0.25, Methods.Method1);
      Console.WriteLine(arr.ToLongString("f7") + "\n");
      
      Console.WriteLine($"Saved - {V1DataArray.SaveBinary("test2", arr)}");
      
      V1DataArray arr2 = new V1DataArray("returned arr", DateTime.Now);
      Console.WriteLine($"Loaded - {V1DataArray.LoadBinary("test2", ref arr2)}");
      Console.WriteLine(arr2.ToLongString("f7"));
    }
    
    static void TestInOutArray2() {
      V1DataArray arr = new V1DataArray("arr", DateTime.Now, 0, 0, 0.25, 0.25, Methods.Method1);
      Console.WriteLine(arr.ToLongString("f7") + "\n");
      
      Console.WriteLine($"Saved - {V1DataArray.SaveBinary("test2", arr)}");
      
      V1DataArray arr2 = new V1DataArray("returned arr", DateTime.Now);
      Console.WriteLine($"Loaded - {V1DataArray.LoadBinary("test2", ref arr2)}");
      Console.WriteLine(arr2.ToLongString("f7"));
    }

    
    
    
    static void Main(string[] args) {
      TestLINQ1();
      //TestLINQ2();   // пустая коллекция
      //TestLINQ3();   // разные крайние значения
      
      //TestInOutArray1();
      //TestInOutArray2();  // пустой массив
      //TestInOutList1();
      //TestInOutList2();   // пустой список

    }
  }
}