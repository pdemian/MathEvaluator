using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Example2
{
    public static void Main(String[] args)
    {
        Console.WriteLine(Calc.Calculate("16 / 2 * (8 - 3 * (4 - 2)) + 1"));
        Console.WriteLine(Calc.Calculate("Log(123)"));
        Console.WriteLine(Calc.Calculate("sin(PI/2)"));
        Console.WriteLine(Calc.Calculate("asin(sin(60 * PI/180)) * 180/PI"));
        Console.WriteLine(Calc.Calculate("-abs -15"));
        Console.WriteLine(Calc.Calculate("-(-phi / pi)"));
        Console.WriteLine(Calc.Calculate("-15 * -15"));
        Console.WriteLine(Calc.Calculate("----15"));
        Console.WriteLine(Calc.Calculate("(15)(1 + 2)"));
        Console.WriteLine(Calc.Calculate("round sin 2pi"));
        Console.WriteLine(Calc.Calculate("5cos pi"));
        Console.WriteLine(Calc.Calculate("abs -150"));
        Console.WriteLine(Calc.Calculate("911 mod 19"));
        Console.WriteLine(Calc.Calculate("log 27 / log 3"));

        Console.Read();
    }
}