using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class ConsoleInput
{
    public static void Main(String[] args)
    {
        //For some reason, the first call to Calc.Calculate() takes a really long time
        //I'm assuming that it's due to the fact that this is a JIT compiled language
        //Anyways, essentially "force" compiling the class here
        Calc.Calculate("1 + 1");

        Console.WriteLine("Type \"exit\" to exit");
        string expr;
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        double val;
        for(;;)
        {
            Console.Write("\nType in an expression: ");
            expr = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(expr)) continue;
            if (expr.Trim().ToLower().Equals("exit")) break;
        
            watch.Start();
            try
            {
                val = Calc.Calculate(expr);
                watch.Stop();
                Console.WriteLine("Answer: " + val);
            }
            catch (Exception ex)
            {
                watch.Stop();
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Time taken: " + watch.ElapsedTicks / 100.0 + " nanoseconds");
            }
            watch.Reset();
        }
    }
}