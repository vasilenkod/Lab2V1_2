using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Lab2
{
    delegate Complex FdlbComplex(double x, double y);

   //методы для присваивания комплексного значения точке
    static class FdlbComplexImpl
    {
        public static Complex F1(double x, double y)
        {
            return new Complex(x + y, x - y);
        }
        
        public static Complex F2(double x, double y)
        {
            return new Complex(x, y);
        }
    }
}
