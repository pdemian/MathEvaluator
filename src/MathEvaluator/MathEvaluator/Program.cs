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
            Queue<Object> o = Calc.Lexxer("3 * log(16 + 5) - 4 / e + - 3.0");

            foreach (Object s in o)
                Console.WriteLine(s.GetType() + " " + s);

            Console.Read();
        }
    }
}
