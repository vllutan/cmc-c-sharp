using System;
using System.Collections.Generic;
using System.Numerics;

namespace lab1_V1Data
{
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

  public delegate Complex FdblComplex(double x, double y);

  abstract class V1Data {
    public string obj { get; }

    public DateTime dt { get; }

    public V1Data(string s, DateTime d) {
      obj = s;
      dt = d;
    }

    public abstract int Count { get; }
    public abstract double AverageValue { get; }
    public abstract string ToLongString(string format);

    public override string ToString() {
      return $"Object {obj}, DateTime {dt}";
    }
  }

  class V1DataList : V1Data {
    private List<DataItem> data { get; }

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
        DataItem di = new DataItem(x, y, F(x,y));
        
        addSum += Add(di) ? 1 : 0;
      }

      return addSum;
    }

    public override int Count {
      get =>  data.Count; 
    }

    public override double AverageValue {
      get {
        double sum = 0;
        foreach (var item in data) sum += item.c.Magnitude;
        return sum / data.Count;
      }
    }

    public override string ToString() {
      return $"Type: V1DataList (List<DataItem>), {base.ToString()}, num of elements: {data.Count}";
    }

    public override string ToLongString(string format) {
      string res = ToString() + "\n";
      foreach (var item in data) {
        res += item.ToLongString(format) + "\n";
      }
      return res;
    }
  }

  class V1DataArray : V1Data {
    public int nodeX { get; }
    public double stepX { get; }
    public int nodeY { get; }
    public double stepY { get; }

    public Complex[,] valueArr { get; }

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
      return $"Type: V1DataArray (Complex[,]), {base.ToString()}" + "\n" +
        $"node: X - {nodeX}, Y - {nodeY}; step: X - {stepX}, Y - {stepY}";
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

  }

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
  }
  
  static class Methods {
    
    public static Complex Method1(double x, double y) {
      Complex new_c = new Complex(x, y);
      return new_c;
    }
    
  }
  
  
  class Program {

    static void Main(string[] args) {
      string format = "f3";
      
      V1DataArray arr1 = new V1DataArray("arr1", DateTime.Now, 3, 3, 0.5, 0.5, Methods.Method1);
      Console.WriteLine(arr1.ToLongString(format));

      V1DataList list1 = (V1DataList) arr1;
      Console.WriteLine(list1.ToLongString(format));
      
      Console.WriteLine($"arr: Count {arr1.Count}, AverageValue {arr1.AverageValue}" + "\n" + 
        $"list: Count {list1.Count}, AverageValue {list1.AverageValue}\n" );

      V1DataArray arr2 = new V1DataArray("arr2", DateTime.Now, 3, 3, 0.25, 0.25, Methods.Method1);
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
  }
}