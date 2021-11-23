using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Linq;
using System.Collections;

namespace Lab2V1_2
{
    struct DataItem
    {
        public double x { get; set; }
        public double y { get; set; }
        public Complex value { get; set; }

        public DataItem(double x, double y, Complex value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
        public string ToLongString(string format)
        {
            return $"({x.ToString(format)}, {y.ToString(format)}), value: {value.ToString(format)}, module: {value.Magnitude.ToString(format)}";
        }
        public override string ToString()
        {
            return $"({x}, {y}), value: {value}";
        }
    }

    delegate Complex FdlbComplex(double x, double y);

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
        public abstract IEnumerator<DataItem> GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    class V1DataList : V1Data
    {
        public List<DataItem> DataList { get; }
        public V1DataList(string object_id, DateTime data) : base(object_id, data)
        {
            DataList = new List<DataItem>();
        }
        public bool Add(DataItem newItem)
        {
            foreach (DataItem Item in DataList)
            {
                if (Item.x == newItem.x && Item.y == newItem.y)
                {
                    return false;
                }
            }
            DataList.Add(newItem);
            return true;
        }

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

        public override int Count
        {
            get { return DataList.Count; }
        }

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


    class V1DataArray : V1Data
    {
        public int xNodes { get; private set; }
        public int yNodes { get; private set; }
        public double xSteps { get; private set; }
        public double ySteps { get; private set; }
        public Complex[,] array { get; private set; }

        public V1DataArray(string object_id, DateTime data) : base(object_id, data)
        {
            xNodes = 0;
            yNodes = 0;
            xSteps = 0;
            ySteps = 0;
            array = new Complex[xNodes, yNodes];
        }
        public V1DataArray(string object_id, DateTime data, int xNodes, int yNodes,
                            double xSteps, double ySteps, FdlbComplex F) : base(object_id, data)
        {
            this.xNodes = xNodes;
            this.yNodes = yNodes;
            this.xSteps = xSteps;
            this.ySteps = ySteps;
            array = new Complex[xNodes, yNodes];
            for (int i = 0; i < xNodes; i++)
            {
                for (int j = 0; j < yNodes; j++)
                {
                    array[i, j] = F(i * xSteps, j * ySteps);
                }
            }
        }
        public override int Count
        {
            get
            {
                return xNodes * yNodes;
            }
        }

        public override double AverageValue
        {
            get
            {
                double averageValue = 0;
                for (int i = 0; i < xNodes; i++)
                {
                    for (int j = 0; j < yNodes; j++)
                    {
                        averageValue += array[i, j].Magnitude;
                    }
                }

                return averageValue / Count;
            }
        }

        public override string ToString()
        {
            return base.ToString() + $" xNodes: {xNodes}, yNodes: {yNodes}, xSteps: {xSteps}, ySteps: {ySteps}";

        }

        public override string ToLongString(string format)
        {
            string str = ToString() + "\n";
            for (int i = 0; i < xNodes; i++)
            {
                for (int j = 0; j < yNodes; j++)
                {
                    DataItem item = new DataItem(i * xNodes, j * yNodes, array[i, j]);
                    str += item.ToLongString(format) + "\n";
                }
            }
            return str;
        }

        public V1DataList ArrayToList()
        {
            V1DataList DataList = new V1DataList(object_id, data);
            for (int i = 0; i < xNodes; i++)
            {
                for (int j = 0; j < yNodes; j++)
                {
                    double x = i * xNodes;
                    double y = j * yNodes;
                    Complex value = array[i, j];
                    DataItem item = new DataItem(x, y, value);
                    DataList.Add(item);
                }
            }
            return DataList;
        }
        public override IEnumerator<DataItem> GetEnumerator()
        {
            for (int i = 0; i < xNodes; i++)
            {
                for (int j = 0; j < yNodes; j++)
                {
                    yield return new DataItem(i * xSteps, j * ySteps, array[i, j]);
                }
            }
        }

        public bool SaveAsText(string filename)
        {
            try
            {
                using (StreamWriter streamWriter = File.CreateText(filename))
                {
                    streamWriter.WriteLine(object_id);
                    streamWriter.WriteLine(data);
                    streamWriter.WriteLine(xNodes);
                    streamWriter.WriteLine(yNodes);
                    streamWriter.WriteLine(xSteps);
                    streamWriter.WriteLine(ySteps);
                    foreach (Complex c in array)
                    {
                        streamWriter.WriteLine(c.Real);
                        streamWriter.WriteLine(c.Imaginary);
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Saving V1DataArray as text have failed: " + e.Message);
                return false;
            }
            return true;
        }
        public bool LoadAsText(string filename)
        {
            try
            {
                using (StreamReader streamReader = new StreamReader(filename))
                {
                    object_id = streamReader.ReadLine();
                    data = DateTime.Parse(streamReader.ReadLine());
                    xNodes = int.Parse(streamReader.ReadLine());
                    yNodes = int.Parse(streamReader.ReadLine());
                    xSteps = double.Parse(streamReader.ReadLine());
                    ySteps = double.Parse(streamReader.ReadLine());
                    array = new Complex[xNodes, yNodes];
                    for (int i = 0; i < xNodes; i++)
                    {
                        for (int j = 0; j < yNodes; j++)
                        {
                            Double real = double.Parse(streamReader.ReadLine());
                            Double imaginary = double.Parse(streamReader.ReadLine());
                            array[i, j] = new Complex(real, imaginary);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Loading V1DataArray as text have failed: " + e.Message);
                return false;
            }
            return true;
        }
    }

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
        public bool Add(V1Data v1Data)
        {
            if (!Contains(v1Data.object_id))
            {
                Collection.Add(v1Data);
                return true;
            }
            return false;
        }


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

    static class Methods
    {
        static public Complex F(double x, double y)
        {
            return new Complex(x + y, x - y);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            FileIOTest();
            LinqTest();
        }
        static void FileIOTest()
        {

            Console.WriteLine("\n\n\n***FileIOTest***\n\n\n");
            string arr_filename = "V1DataArray_data.txt";
            string list_filename = "V1DataList_data";

            //V1DataArray save/load
            V1DataArray arr_saved = new V1DataArray("arr_saved", DateTime.Today, 6, 4, 0.5, 0.7, Methods.F);
            Console.WriteLine("Saved V1DataArray:\n" + arr_saved.ToLongString("F") + "\n");
            arr_saved.SaveAsText(arr_filename);

            V1DataArray arr_loaded = new V1DataArray("arr_loaded", DateTime.Today);
            arr_loaded.LoadAsText(arr_filename);
            Console.WriteLine("Loaded V1DataArray:\n" + arr_loaded.ToLongString("F") + "\n");

            //V1DataList save/load
            V1DataList list_saved = new V1DataList("list_saved", DateTime.Today);
            list_saved = arr_loaded.ArrayToList();
            list_saved.Add(new DataItem(0.5, 0.5, new Complex(1, 2)));
            list_saved.Add(new DataItem(0.6, 0.7, new Complex(1, 1)));
            Console.WriteLine("Saved V1DataList:\n" + list_saved.ToLongString("F") + "\n");
            list_saved.SaveBinary(list_filename);

            V1DataList list_loaded = new V1DataList("list_loaded", new DateTime(1, 1, 1));
            list_loaded.LoadBinary(list_filename);
            Console.WriteLine("Loaded V1DataList:\n" + list_loaded.ToLongString("F") + "\n");

            //empty V1DataArray save/load
            V1DataArray arr_empty = new V1DataArray("arr_empty", new DateTime(1, 1, 1));
            Console.WriteLine("Saved V1DataArray empty: " + arr_empty.ToLongString("F"));
            arr_empty.SaveAsText(arr_filename);
            arr_empty.LoadAsText(arr_filename);
            Console.WriteLine("Loaded V1DataArray empty: " + arr_empty.ToLongString("F"));

            //empty V1DataList save/load
            V1DataList list_empty = new V1DataList("list_empty", new DateTime(1, 1, 1));
            Console.WriteLine("Saved V1DataList empty: " + list_empty.ToLongString("F"));
            list_empty.SaveBinary(list_filename);
            list_empty.LoadBinary(list_filename);
            Console.WriteLine("Loaded V1DataList empty: " + list_empty.ToLongString("F") );


        }
        static void LinqTest()
        {
            Console.WriteLine("\n\n\n***LinqTest***\n\n\n");
            V1MainCollection myCollection = new V1MainCollection();
            myCollection.Add(new V1DataArray("V1DataArray", new DateTime(1970, 1, 1), 2, 1, 0.5, 1, Methods.F));
            myCollection.Add(new V1DataArray("V1DataArray empty", DateTime.Today));
            myCollection.Add(new V1DataList("V1DataList empty1", new DateTime(2020, 1, 1)));
            myCollection.Add(new V1DataList("V1DataList empty2", new DateTime(2001, 9, 13)));


            V1DataList list = new V1DataList("V1DataList", new DateTime(2011, 11, 11));
            list.Add(new DataItem(0.2, 0.1, new Complex(1, 2)));
            list.Add(new DataItem(4.5, 1.3, new Complex(1, 1)));
            list.Add(new DataItem(0.8, 2.3, new Complex(2, 5)));
            myCollection.Add(list);

            Console.WriteLine($"MinDate:\n{myCollection.MinDate}\n");

            Console.WriteLine("SortByAverage:");
            int i = 0;
            foreach (var x in myCollection.SortByAverage)
            {
                i++;
                Console.WriteLine($"{i}\n{x.ToLongString("F")}");
            }

            Console.WriteLine("MaxCount:");
            i = 0;
            foreach (var x in myCollection.MaxCount)
            {
                i++;
                Console.WriteLine($"{i}\n{x.ToLongString("F")}");
            }
        }
    }

}