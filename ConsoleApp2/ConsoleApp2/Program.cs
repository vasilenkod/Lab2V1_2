using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Linq;
using System.Collections;

namespace Lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            FileIOTest();
            LinqTest();
        }

        //тесты для записи\чтениия коллекция в файл
        static void FileIOTest()
        {

            Console.WriteLine("\n\n\n***FileIOTest***\n\n\n");
            string arr_filename = "V1DataArray_data.txt";
            string list_filename = "V1DataList_data";

            //V1DataArray save/load
            V1DataArray arr_saved = new V1DataArray("arr_saved", DateTime.Today, 6, 4, 0.5, 0.7, FdlbComplexImpl.F1);
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
        //тесты для методов Count и SortByAverage
        static void LinqTest()
        {
            Console.WriteLine("\n\n\n***LinqTest***\n\n\n");
            V1MainCollection myCollection = new V1MainCollection();
            myCollection.Add(new V1DataArray("V1DataArray", new DateTime(1970, 1, 1), 2, 1, 0.5, 1, FdlbComplexImpl.F1));
            myCollection.Add(new V1DataArray("V1DataArray empty", DateTime.Today));
            V1DataList list1 = new V1DataList("V1DataList non-empty", new DateTime(2020, 1, 1));
            list1.AddDefaults(3, FdlbComplexImpl.F1);
            myCollection.Add(list1);
            myCollection.Add(new V1DataList("V1DataList empty2", new DateTime(2001, 9, 13)));


            V1DataList list = new V1DataList("V1DataList", new DateTime(2011, 11, 11));
            list.AddDefaults(7, FdlbComplexImpl.F1);
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