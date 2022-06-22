using System;
using System.Collections.Generic;
using System.Collections;

namespace Lab2
{
    //Коллекция в которой будут храниться наши точки, измеренные в ходе эксперимента
    abstract class V1Data : IEnumerable<DataItem>
    {
        public string object_id { get; protected set; }
        public DateTime data { get; protected set; }

        public V1Data(string object_id, DateTime data)
        {
            this.object_id = object_id;
            this.data = data;
        }
        public abstract int Count { get; }
        public abstract double AverageValue { get; }
        public abstract string ToLongString(string format);
        public override string ToString()
        {
            return $"id: {object_id}, date: {data}";
        }
        //По коллекции можно будет итерироваться
        public abstract IEnumerator<DataItem> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
