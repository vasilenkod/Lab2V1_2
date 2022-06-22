using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Collections;

namespace Lab2

{
    //Коллекция, хранящая данные измерений на двумерной прямоугольной сетке, с равномерным шагом
    // по Ox, и Oy в двумерном прямоугольном массиве размера [xNodes, yNodes]
    class V1DataArray : V1Data
    {

        public int xNodes { get; private set; }
        public int yNodes { get; private set; }
        //шаг сетки по Ox
        public double xSteps { get; private set; }
        //шаг сетки по Oy
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
        //заполняет массив точками
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

        //общее количество измеренных точек
        public override int Count
        {
            get
            {
                return xNodes * yNodes;
            }
        }
        
        //среднее значение всех точек
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
        //переводит эту коллецкию в одномерный список
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
        //запись коллекции в текстовый файл
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
        //загрузка коллекции из текстого файла
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
}
