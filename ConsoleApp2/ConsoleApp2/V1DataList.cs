using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Collections;

namespace Lab2

{
    //Коллекция, хранящая данные измерений в списке
    class V1DataList : V1Data
    {
        public List<DataItem> DataList { get; }
        public V1DataList(string object_id, DateTime data) : base(object_id, data)
        {
            DataList = new List<DataItem>();
        }
        //добавляет в коллекцию различные элементы
        public bool Add(DataItem newItem)
        {
            bool Equality(DataItem item)
            {
                const double eps = 0.00001;
                return Math.Abs(newItem.x - item.x) <= eps && 
                        Math.Abs(newItem.y - item.y) <= eps;
            }
            foreach (DataItem Item in DataList)
            {
                if (Equality(newItem) == false)
                {
                    return false;
                }
            }
            DataList.Add(newItem);
            return true;
        }
        //добавляет в коллекцию n точек со значениями, вычисленными по правилу F
        public int AddDefaults(int nItems, FdlbComplex F)
        {
            int count = 0;
            for (int i = 0; i < nItems; i++)
            {
                double x = i * 1.42;
                double y = i * 1.11;
                DataItem newItem = new DataItem(x, y, F(x, y));
                if (Add(newItem))
                {
                    count++;
                }
            }
            return count;
        }

        //число измеренных точек
        public override int Count
        {
            get { return DataList.Count; }
        }

        //среднее значение точек
        public override double AverageValue
        {
            get
            {
                if (Count == 0)
                {
                    return 0;
                }
                double averageValue = 0;
                foreach (DataItem item in DataList)
                {
                    averageValue += item.value.Magnitude;
                }
                return averageValue / Count;
            }
        }

        public override string ToLongString(string format)
        {
            string str = ToString() + "\n";
            foreach (DataItem item in DataList)
            {
                str += item.ToLongString(format) + "\n";
            }
            return str;
        }
        public override string ToString()
        {
            return base.ToString() + $" Count: {Count}";
        }

        public override IEnumerator<DataItem> GetEnumerator()
        {
            return DataList.GetEnumerator();
        }
        //cохраняет коллекцую в бинарный файл
        public bool SaveBinary(string filename)
        {
            try
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(filename, FileMode.OpenOrCreate)))
                {
                    binaryWriter.Write(object_id);
                    binaryWriter.Write(data.ToString());
                    binaryWriter.Write(Count);
                    foreach (DataItem item in DataList)
                    {
                        binaryWriter.Write(item.x);
                        binaryWriter.Write(item.y);
                        binaryWriter.Write(item.value.Real);
                        binaryWriter.Write(item.value.Imaginary);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Saving V1DataList as binary have failed: " + e.Message);
                return false;
            }
            return true;
        }
        //загружает коллекцию из бинарного файла
        public bool LoadBinary(string filename)
        {
            try
            {
                using (BinaryReader binaryReader = new BinaryReader(File.Open(filename, FileMode.Open)))
                {
                    object_id = binaryReader.ReadString();
                    data = DateTime.Parse(binaryReader.ReadString());
                    int count = binaryReader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        DataItem newitem = new DataItem(binaryReader.ReadDouble(), binaryReader.ReadDouble(),
                                                        new Complex(binaryReader.ReadDouble(), binaryReader.ReadDouble()));
                        this.Add(newitem);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Loading V1DataList as binary have failed" + e.Message);
                return false;
            }
            return true;
        }

    }
}
