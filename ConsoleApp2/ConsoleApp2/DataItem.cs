using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;


namespace Lab2
{
    //хранит координату точки (x, y) и комплексное значение точки value
    class DataItem
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
}
