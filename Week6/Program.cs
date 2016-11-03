using System;
using System.Linq.Expressions;

namespace Week6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Expression<Func<double, double>> f = x => (x + x + 2 + 5);
            Expression<Func<double, double>> df = f.Differentiate();

            Console.WriteLine("f  = {0}", f);
            Console.WriteLine("df = {0}", df);

            Func<double, double> compiled = df.Compile();
            double result = compiled.Invoke(12);
            Console.WriteLine(result);
        }
    }
}
