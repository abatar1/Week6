using System;
using System.Linq.Expressions;

namespace Week6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Expression<Func<double, double>> f = x => x;
            Expression<Func<double, double>> df = f.Differentiate();

            Console.WriteLine("f  = {0}", f);   //f  = x => ((10 + Sin(x)) * x)
            Console.WriteLine("df = {0}", df);  //df = x => ((10 + Sin(x)) + (Cos(x) * x))

            Func<double, double> compiled = df.Compile();
            double result = compiled.Invoke(12);
            Console.WriteLine(result);
        }
    }
}
