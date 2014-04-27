using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
    This question dates back to a freshman exam question I had.
    Essentially, the goal was to calculate all combinations of:
    4 op 4 op 4 op 4
    where op is some operator [+,-,/,*].
    During an exam, of course, I did not have access to this library,
    and thus relied on the good old copy-and-paste-and-change-
    a-character-one-at-a-time method
    
    It's 64 total combinations if you were curious, although only 
    24 distinct results when calculated.
*/

class SimpleProblem
{
    public static void Main(String[] args)
    {
        string[] ops = { "+", "-", "/", "*" };
        SortedSet<double> set = new SortedSet<double>();
        List<string> list = new List<string>();

        //get all combinations
        for (int a = 0; a < ops.Length; a++)
            for (int b = 0; b < ops.Length; b++)
                for (int c = 0; c < ops.Length; c++)
                    list.Add("4" + ops[a] + "4" + ops[b] + "4" + ops[c] + "4");

        //calculate each combination and add it to a sorted set
        foreach (string s in list)
            set.Add(Calc.Calculate(s));

        //print out the sorted set
        foreach (double d in set)
            Console.WriteLine(d);

        Console.Read();
    }
}