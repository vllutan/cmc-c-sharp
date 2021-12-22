using System;
using System.Collections.Generic;
using System.Collections;

namespace lab1_V1Data
{
    // --------------------------------------------   V1Data   --------------------------------------------

    abstract class V1Data : IEnumerable<DataItem>
    {
        public string obj { get; protected set; }

        public DateTime dt { get; protected set; }

        public V1Data(string s, DateTime d)
        {
            obj = s;
            dt = d;
        }

        public abstract int Count { get; }
        public abstract double AverageValue { get; }
        public abstract string ToLongString(string format);
        public override string ToString()
        {
            return $"Object {obj}; DateTime {dt}";
        }

        public abstract IEnumerator<DataItem> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}