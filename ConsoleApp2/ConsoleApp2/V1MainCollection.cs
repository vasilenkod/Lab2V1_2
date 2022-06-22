using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Linq;
using System.Collections;


namespace Lab2
{
    //Коллекция, хранящая точки, измеренными разными способами, т.е
    // через V1DataList или V1DataArray
    class V1MainCollection
    {
        private List<V1Data> Collection = new List<V1Data>();
        public int Count()
        {
            return Collection.Count;
        }
        public V1Data this[int index]
        {
            get
            {
                return Collection[index];
            }
        }
        public bool Contains(string ID)
        {
            foreach (V1Data Data in Collection)
            {
                if (Data.object_id == ID)
                {
                    return true;
                }
            }
            return false;
        }
        //добавляет в наш список коллекций очередную коллекцию
        public bool Add(V1Data v1Data)
        {
            if (!Contains(v1Data.object_id))
            {
                Collection.Add(v1Data);
                return true;
            }
            return false;
        }

        //возвращает дату самого первого измерения, если оно есть
        public DateTime? MinDate
        {
            get
            {
                if (Collection.Count == 0)
                {
                    return null;
                }
                return Collection.Min(x => x.data);
            }
        }

        //возвращает коллекцию\коллекции с наибольшим числом элементов
        public IEnumerable<V1Data> MaxCount
        {
            get
            {
                if (Collection.Count == 0)
                {
                    return null;
                }
                var max_count = Collection.Max(x => x.Count);
                return Collection.Where(x => x.Count == max_count);
            }
        }
        //возвращает коллекции в порядке убывания среднего арифметического значения
        public IEnumerable<V1Data> SortByAverage
        {
            get
            {
                if (Collection.Count() == 0)
                {
                    return null;
                }
                return Collection.Where(x => x is V1DataList).OrderByDescending(x => x.AverageValue);
            }
        }
        public string ToLongString(string format)
        {
            string str = "";
            foreach (V1Data item in Collection)
            {
                str += item.ToLongString(format) + "\n";
            }
            return str;
        }

        public override string ToString()
        {
            string str = "";
            foreach (V1Data item in Collection)
            {
                str += item.ToString() + "\n";
            }
            return str;
        }

    }
}
