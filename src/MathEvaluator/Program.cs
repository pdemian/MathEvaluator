using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEvaluator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Calc.Calculate("3 * log(4)"));
            Console.Read();
        }
    }
}
