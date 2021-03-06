﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Calc
{
    private static class Symbols
    {
        public const char LEFT_PAREN  = '(';
        public const char RIGHT_PAREN = ')';
        public const char EXP = '^';
        public const char MUL = '*';
        public const char DIV = '/';
        public const char MOD = '%';
        public const char ADD = '+';
        public const char SUB = '-';
    }

    private static class Functions
    {
        public const string LOG   = "log";
        public const string LN    = "ln";
        public const string SQRT  = "sqrt";
        public const string NSQRT = "nsqrt";
        public const string SIN   = "sin";
        public const string COS   = "cos";
        public const string TAN   = "tan";
        public const string ASIN  = "asin";
        public const string ACOS  = "acos";
        public const string ATAN  = "atan";
        public const string FLOOR = "floor";
        public const string CEIL  = "ceil";
        public const string ROUND = "round";
        public const string ABS   = "abs";
        public const string MOD   = "mod";
    }

    private static class Constants
    {
        public const string PI  = "pi";
        public const string E   = "e";
        public const string PHI = "phi";

        public const double PI_VALUE  = 3.141592653589793238;
        public const double E_VALUE   = 2.718281828459045235;
        public const double PHI_VALUE = 1.618033988749894848;
    }

    //Evaluator evaluates the tokens into a doubleing-point number
    private static double Evaluator(Queue<object> str)
    {
        Stack<double> Stack = new Stack<double>();

        for (int TokenCounter = 0; TokenCounter < str.Count; TokenCounter++)
        {
            //add numbers to the stack
            #region numbers
            if (str.ElementAt(TokenCounter) is double)
            {
                Stack.Push((double)str.ElementAt(TokenCounter));
            }
            #endregion

            //calculates the stack if there is an operator in the queue
            #region operators
            else
            {
                object current = str.ElementAt(TokenCounter);
                double temp;

                if (current is string)
                {
                    if(current.Equals(Functions.NSQRT))
                    {
                        if (Stack.Count < 2) throw new Exception("Maldeformed expression");
                        Stack.Push(Math.Pow(Stack.Pop(), 1/Stack.Pop()));
                    }
                    else if (current.Equals(Functions.MOD))
                    {
                        if (Stack.Count < 2) throw new Exception("Maldeformed expression");
                        temp = Stack.Pop();
                        if(temp == 0.0) throw new DivideByZeroException("Division by zero in expression");
                        Stack.Push(Stack.Pop() % temp);
                    }
                    else
                    {
                        if (Stack.Count < 1) throw new Exception("Maldeformed expression");
                        temp = Stack.Pop();

                        //perform the function
                        if (current.Equals(Functions.LOG)) Stack.Push(Math.Log10(temp));
                        else if (current.Equals(Functions.LN)) Stack.Push(Math.Log(temp));
                        else if (current.Equals(Functions.SQRT)) Stack.Push(Math.Sqrt(temp));
                        else if (current.Equals(Functions.SIN)) Stack.Push(Math.Sin(temp));
                        else if (current.Equals(Functions.COS)) Stack.Push(Math.Cos(temp));
                        else if (current.Equals(Functions.TAN)) Stack.Push(Math.Tan(temp));
                        else if (current.Equals(Functions.ASIN)) Stack.Push(Math.Asin(temp));
                        else if (current.Equals(Functions.ACOS)) Stack.Push(Math.Acos(temp));
                        else if (current.Equals(Functions.ATAN)) Stack.Push(Math.Atan(temp));
                        else if (current.Equals(Functions.FLOOR)) Stack.Push(Math.Floor(temp));
                        else if (current.Equals(Functions.CEIL)) Stack.Push(Math.Ceiling(temp));
                        else if (current.Equals(Functions.ROUND)) Stack.Push(Math.Round(temp));
                        else Stack.Push(Math.Abs(temp));
                    }
                }
                else
                {
                    //all symbol operators require 2 items on the stack
                    if(Stack.Count < 2) throw new Exception("Maldeformed expression");

                    temp = (double)Stack.Pop();
                    if (current.Equals(Symbols.ADD)) temp += Stack.Pop();
                    else if (current.Equals(Symbols.SUB)) temp = Stack.Pop() - temp;
                    else if (current.Equals(Symbols.MUL)) temp *= Stack.Pop();
                    else if(current.Equals(Symbols.DIV)) { if(temp == 0.0) throw new DivideByZeroException("Division by zero in expression"); else temp = Stack.Pop() / temp; }
                    else if(current.Equals(Symbols.MOD)) { if(temp == 0.0) throw new DivideByZeroException("Division by zero in expression"); else temp = Stack.Pop() % temp; }
                    else temp = Math.Pow(Stack.Pop(), temp);

                    Stack.Push(temp);
                }
            }
            #endregion
        }

        if (Stack.Count > 1) throw new Exception("Maldeformed expression");

        return (double)Stack.Pop();
    }

    //ToRPN converts the tokens into an RPN queue (using the Shunting Yard Algorithm)
    private static Queue<object> ToRPN(Queue<object> str)
    {
        Stack<object> Stack = new Stack<object>();
        Queue<object> Queue = new Queue<object>();

        for (int TokenCounter = 0; TokenCounter < str.Count; TokenCounter++)
        {
            //if the token is a number, add it to the output queue.
            #region Numbers
            if (str.ElementAt(TokenCounter) is double) Queue.Enqueue(str.ElementAt(TokenCounter));
            #endregion

            //if the token is a left parentheses, push it onto the stack
            #region Left Paren
            else if (str.ElementAt(TokenCounter).Equals(Symbols.LEFT_PAREN))
            {
                //if it's something like "15(5 + 2)" the 15 should be multiplied to make the expression "15 * (5 + 2)"
                //fix this later, but the only way I can think of doing this is going back, appending a '*', and continuing
                if (TokenCounter >= 1 && (str.ElementAt(TokenCounter - 1) is double || str.ElementAt(TokenCounter - 1).Equals(Symbols.RIGHT_PAREN)))
                {
                    List<object> list = new List<object>(str);
                    list.Insert(TokenCounter, Symbols.MUL);
                    str = new Queue<object>(list);
                    TokenCounter--;
                }
                else Stack.Push(Symbols.LEFT_PAREN);
            }
            #endregion
            #region Right Paren
            else if (str.ElementAt(TokenCounter).Equals(Symbols.RIGHT_PAREN))
            {
                bool IsParenMatched = false;

                //Until the token at the top of the stack is a left parentheses, pop operators off the stack onto the output queue.
                while (Stack.Count > 0 && !IsParenMatched)
                {
                    //Pop the left parentheses from the stack, but not onto the output queue.
                    if (Stack.Peek().Equals(Symbols.LEFT_PAREN))
                    {
                        Stack.Pop();
                        IsParenMatched = true;
                    }
                    else Queue.Enqueue(Stack.Pop());
                }

                //If the stack runs out without finding a left parentheses, then there are mismatched parentheses.
                if (!IsParenMatched) throw new Exception("Parentheses mismatch");
            }
            #endregion

            #region characters
            else
            {
                object current = str.ElementAt(TokenCounter);

                //Use the shunting yard algorithm to turn this into an RPN queue

                /* HIGHEST PRECEDENCE */
                if (current.Equals(Symbols.MOD) || current.Equals(Symbols.MUL) || current.Equals(Symbols.DIV))
                {
                    while (Stack.Count > 0 &&
                        !Stack.Peek().Equals(Symbols.LEFT_PAREN) &&
                        !(Stack.Peek().Equals(Symbols.ADD) || Stack.Peek().Equals(Symbols.SUB)))
                    {
                        Queue.Enqueue(Stack.Pop());
                    }

                }

                /* LOWEST PRECEDENCE */
                if (current.Equals(Symbols.ADD) || current.Equals(Symbols.SUB))
                {
                    while (Stack.Count > 0 &&
                        !Stack.Peek().Equals(Symbols.LEFT_PAREN))
                    {
                        Queue.Enqueue(Stack.Pop());
                    }
                }

                //push the current operator onto the stack
                Stack.Push(current);
            }
            #endregion
        }
        //When there are no more tokens to read:
        //While there are still operator tokens in the stack:
        //        If the operator token on the top of the stack is a parentheses, then there are mismatched parentheses.
        //        Pop the operator onto the output queue.
        while (Stack.Count > 0)
        {
            if (Stack.Peek().Equals(Symbols.LEFT_PAREN)) throw new Exception("Parentheses mismatch");
            Queue.Enqueue(Stack.Pop());
        }


        return Queue;
    }

    //Parser tokenizes the input string for evaluating
    private static Queue<object> Parser(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) throw new ArgumentException("Empty string");

        Queue<object> Queue = new Queue<object>();
        //remove multiple whitespace (only a single whitespace character allowed)
        StringBuilder Stringbuilder = new StringBuilder(System.Text.RegularExpressions.Regex.Replace(str, @"\s\s+", " ")); //new StringBuilder(new string(str.Where(x => !Char.IsWhiteSpace(x)).ToArray()));

        for (int TokenCounter = 0; TokenCounter < Stringbuilder.Length; TokenCounter++)
        {
            //parse ints/doubles
            #region numbers
            if (Stringbuilder[TokenCounter] >= '0' && Stringbuilder[TokenCounter] <= '9')
            {
                string ParseDigits = Stringbuilder[TokenCounter].ToString();
                bool HasDecimal = false;

                //there are some errors with digits that have a length of 1 at the end of the string. Note: clean this up
                if (TokenCounter + 1 < Stringbuilder.Length)
                {
                    for (; Stringbuilder[TokenCounter+1] == '.' || (Stringbuilder[TokenCounter+1] >= '0' && Stringbuilder[TokenCounter+1] <= '9');)
                    {
                        TokenCounter++;
                        if (Stringbuilder[TokenCounter] == '.' && !HasDecimal)
                        {
                            ParseDigits += '.';
                            HasDecimal = true;
                        }
                        else if (Stringbuilder[TokenCounter] == '.' && HasDecimal) throw new Exception("Unmatched decimal point at position '" + TokenCounter + "'");
                        else ParseDigits += Stringbuilder[TokenCounter];

                        if (TokenCounter + 1 >= Stringbuilder.Length) break;
                    }
                }
                Queue.Enqueue(double.Parse(ParseDigits));
            }
            #endregion
            //parse words
            #region words
            else if (Char.IsLetter(Stringbuilder[TokenCounter]))
            {
                //get the entire word
                StringBuilder sb = new StringBuilder();
                while (TokenCounter < Stringbuilder.Length && Char.IsLetter(Stringbuilder[TokenCounter]))
                    sb.Append(Stringbuilder[TokenCounter++]);

                //temp bug fix. eats up next character if uncommented
                TokenCounter--;

                string word = sb.ToString().ToLower();
                switch (word)
                {
                    //functions
                    case Functions.LOG:
                    case Functions.LN:
                    case Functions.SQRT:
                    case Functions.NSQRT:
                    case Functions.SIN:
                    case Functions.COS:
                    case Functions.TAN:
                    case Functions.ASIN:
                    case Functions.ACOS:
                    case Functions.ATAN:
                    case Functions.FLOOR:
                    case Functions.CEIL:
                    case Functions.ROUND:
                    case Functions.ABS:
                    case Functions.MOD:
                        Queue.Enqueue(word);
                        break;
                    
                    //constants
                    case Constants.PI:
                    case Constants.E:
                    case Constants.PHI:
                        Queue.Enqueue(word);
                        break;
                    default:
                        throw new Exception("Unknown function or constant: \"" + word + "\"");
                }
            }
            #endregion
            //parse symbols
            #region symbols
            else
            {
                switch (Stringbuilder[TokenCounter])
                {
                    case Symbols.LEFT_PAREN:
                        Queue.Enqueue(Symbols.LEFT_PAREN);
                        continue;
                    case Symbols.RIGHT_PAREN:
                        Queue.Enqueue(Symbols.RIGHT_PAREN);
                        continue;
                    case Symbols.MUL:
                        Queue.Enqueue(Symbols.MUL);
                        continue;
                    case Symbols.EXP:
                        Queue.Enqueue(Symbols.EXP);
                        continue;
                    case Symbols.DIV:
                        Queue.Enqueue(Symbols.DIV);
                        continue;
                    case Symbols.MOD:
                        Queue.Enqueue(Symbols.MOD);
                        continue;
                    case Symbols.ADD:
                        Queue.Enqueue(Symbols.ADD);
                        continue;
                    case Symbols.SUB:
                        Queue.Enqueue(Symbols.SUB);
                        continue;
                }
                if (Char.IsWhiteSpace(Stringbuilder[TokenCounter])) continue;
                throw new Exception("Unknown symbol: '" + Stringbuilder[TokenCounter] + "'");
            }
            #endregion
        }


        //fix negatives, if any
        for (int i = 1; i < Queue.Count; i++)
        {
            //two tokens in a row and the second token being '-' means it a negative number
            bool negate = false;
            //if we're at the beginning
            if (i == 1)
            {
                if (Queue.ElementAt(i - 1).Equals(Symbols.SUB)) negate = true;
            }
            //otherwise
            else
            {
                if (Queue.ElementAt(i - 1).Equals(Symbols.SUB) && !(Queue.ElementAt(i - 2) is double)) negate = true;
            }


            if (negate)
            {
                if (Queue.ElementAt(i) is double)
                {
                    //negate the double
                    List<object> list = new List<object>(Queue);
                    list[i] = -(double)list[i];
                    list.RemoveAt(i - 1);
                    Queue = new Queue<object>(list);
                }
                else
                {
                    //place parentheses around expression and multiply by -1
                    int iterator;
                    int paren_count = 0;
                    for (iterator = i; iterator < Queue.Count && paren_count >= 0; iterator++)
                    {
                        if (Queue.ElementAt(i).Equals(Symbols.LEFT_PAREN)) paren_count++;
                        else if (Queue.ElementAt(i).Equals(Symbols.RIGHT_PAREN)) paren_count--;
                    }

                    List<object> list = new List<object>(Queue);

                    //insert (-1 * ( ... ) ) to avoid any ambiguity
                    list.RemoveAt(i-1); //remove '-'
                    list.Insert(i-1, Symbols.LEFT_PAREN);
                    list.Insert(i, -1.0);
                    list.Insert(i+1, Symbols.MUL);
                    list.Insert(i+2, Symbols.LEFT_PAREN);
                    list.Insert(iterator+3, Symbols.RIGHT_PAREN);
                    list.Insert(iterator+4, Symbols.RIGHT_PAREN);

                    Queue = new Queue<object>(list);
                }
            }
        }

        //handle implicit multiplication 
        //eg, 2sin(2pi)
        List<object> List = new List<object>(Queue);
        for (int i = 1; i < List.Count; i++)
        {
            if (List.ElementAt(i) is string && 
                !(List.ElementAt(i).Equals(Functions.MOD) || List.ElementAt(i).Equals(Functions.NSQRT)) && 
                List.ElementAt(i - 1) is double)
            {
                List.Insert(i, Symbols.MUL);
            }
        }

        //convert constants to their actual values
        List = List.Select(x =>
        {
            if (x is string)
            {
                if (x.Equals(Constants.PI)) return Constants.PI_VALUE;
                else if (x.Equals(Constants.E)) return Constants.E_VALUE;
                else if (x.Equals(Constants.PHI)) return Constants.PHI_VALUE;
            }
            return x;
        }).ToList();

        return new Queue<object>(List);
    }

    /// <summary>
    /// Calculates a mathematical expression
    /// </summary>
    /// <param name="str">A valid mathematical expression</param>
    /// <returns>The result of the calculation</returns>
    public static double Calculate(string str)
    {
        try
        {
            return Evaluator(ToRPN(Parser(str)));
        }
        catch (Exception)
        {
            throw;
        }
    }
}

