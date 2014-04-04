using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Example
{
    public static void Main(String[] args)
    {
        Console.WriteLine("Calculated: " + Calc.Calculate("16 / 2 * (8 - 3 * (4 - 2)) + 1"));
        Console.WriteLine("Actual: " + (16 / 2 * (8 - 3 * (4 - 2)) + 1));

        Console.WriteLine();

        Console.WriteLine("Calculated: " + Calc.Calculate("1600 * 2 / 3 + 1 - 8 ^ 2 % 1"));
        Console.WriteLine("Actual: " + (1600 * 2 / 3.0 + 1 - Math.Pow(8,2) % 1));

        Console.WriteLine();

        try
        {
            //this should throw an exception
            Console.WriteLine(Calc.Calculate("1 / 0"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.Read();
    }
}