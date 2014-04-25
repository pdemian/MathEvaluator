using System;
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
    }

    private static class Constants
    {
        public const string PI  = "pi";
        public const string E   = "e";
        public const string PHI = "phi";

        public const double PI_VALUE  = 3.14159265358979323846264338327950288419716939937510582097494459230781640628620899862803482534211706798;
        public const double E_VALUE   = 2.71828182845904523536028747135266249775724709369995957496696762772407663035354759457138217852516642742;
        public const double PHI_VALUE = 1.61803398874989484820458683436563811772030917980576286213544862270526046281890244970720720418939113748;
    }

    //Evaluator evaluates the tokens into a doubleing-point number
    private static double Evaluator(Queue<Object> str)
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
                Object current = str.ElementAt(TokenCounter);
                double temp;

                if (current is string)
                {
                    if(current.Equals(Functions.NSQRT))
                    {
                        if (Stack.Count < 2) throw new Exception("Maldeformed expression");
                        Stack.Push(Math.Pow(Stack.Pop(), 1/Stack.Pop()));
                    }
                    else
                    {
                        if (Stack.Count < 1) throw new Exception("Maldeformed expression");
                        temp = Stack.Pop();

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
                    else if (current.Equals(Symbols.SUB)) temp -= Stack.Pop();
                    else if (current.Equals(Symbols.MUL)) temp *= Stack.Pop();
                    else if(current.Equals(Symbols.DIV)) { if(temp == 0.0) throw new Exception("Division by zero"); else temp = Stack.Pop() / temp; }
                    else if(current.Equals(Symbols.MOD)) { if(temp == 0.0) throw new Exception("Division by zero"); else temp = Stack.Pop() % temp; }
                    else temp = Math.Pow(Stack.Pop(), temp);

                    Stack.Push(temp);
                }
            }
            #endregion
        }

        if (Stack.Count > 1) throw new Exception("too many values entered");

        return (double)Stack.Pop();
    }

    //ToRPN converts the tokens into an RPN queue (using the Shunting Yard Algorithm)
    private static Queue<Object> ToRPN(Queue<Object> str)
    {
        Stack<Object> Stack = new Stack<Object>();
        Queue<Object> Temp_queue = new Queue<Object>();

        for (int TokenCounter = 0; TokenCounter < str.Count; TokenCounter++)
        {
            //if the token is a number, add it to the output queue.
            #region Numbers
            if (str.ElementAt(TokenCounter) is double) Temp_queue.Enqueue(str.ElementAt(TokenCounter));
            #endregion

            //if the token is a left parenthesis, push it onto the stack
            #region Left Paren
            else if (str.ElementAt(TokenCounter).Equals(Symbols.LEFT_PAREN)) Stack.Push(Symbols.LEFT_PAREN);
            #endregion
            #region Right Paren
            else if (str.ElementAt(TokenCounter).Equals(Symbols.RIGHT_PAREN))
            {
                bool IsParenMatched = false;

                //Until the token at the top of the stack is a left parenthesis, pop operators off the stack onto the output queue.
                while (Stack.Count > 0 && !IsParenMatched)
                {
                    //Pop the left parenthesis from the stack, but not onto the output queue.
                    if (Stack.Peek().Equals(Symbols.LEFT_PAREN))
                    {
                        Stack.Pop();
                        IsParenMatched = true;
                    }
                    else Temp_queue.Enqueue(Stack.Pop());
                }

                //If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                if (!IsParenMatched) throw new Exception("Parentheses mismatch");
            }
            #endregion

            #region characters
            else
            {
                Object current = str.ElementAt(TokenCounter);

                /* HIGHEST PRECEDENCE */
                if(current.Equals(Symbols.MOD) || current.Equals(Symbols.MUL) || current.Equals(Symbols.DIV))
                {
                    while (Stack.Count > 0 &&
                        !Stack.Peek().Equals(Symbols.LEFT_PAREN) &&
                        !(Stack.Peek().Equals(Symbols.ADD) || Stack.Peek().Equals(Symbols.SUB)))
                    {
                        Temp_queue.Enqueue(Stack.Pop());
                    }
                    
                }

                /* LOWEST PRECEDENCE */
                if (current.Equals(Symbols.ADD) || current.Equals(Symbols.SUB))
                {
                    while (Stack.Count > 0 &&
                        !Stack.Peek().Equals(Symbols.LEFT_PAREN))
                    {
                        Temp_queue.Enqueue(Stack.Pop());
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
            Temp_queue.Enqueue(Stack.Pop());
        }


        return Temp_queue;
    }

    //Parser tokenizes the input string for evaluating
    private static Queue<Object> Parser(string str)
    {
        if (str == null || str.Length == 0) throw new Exception("Empty string");

        Queue<Object> Temp_queue = new Queue<object>();
        StringBuilder Temp_string = new StringBuilder(str);
        
        //used for negative numbers. If there were two operators in a row (eg, "1 * - 1"), the second minus is a negation sign
        int SymbolsInARow = 0;

        for (int TokenCounter = 0; TokenCounter < Temp_string.Length; TokenCounter++)
        {
            //parse ints/doubles
            #region numbers
            if (Temp_string[TokenCounter] >= '0' && Temp_string[TokenCounter] <= '9')
            {
                string ParseDigits = Temp_string[TokenCounter].ToString();
                bool HasDecimal = false;

                //there are some errors with digits that have a length of 1 at the end of the string. Note: clean this up
                if (TokenCounter + 1 < Temp_string.Length)
                {
                    for (; Temp_string[TokenCounter+1] == '.' || (Temp_string[TokenCounter+1] >= '0' && Temp_string[TokenCounter+1] <= '9');)
                    {
                        TokenCounter++;
                        if (Temp_string[TokenCounter] == '.' && !HasDecimal)
                        {
                            ParseDigits += '.';
                            HasDecimal = true;
                        }
                        else if (Temp_string[TokenCounter] == '.' && HasDecimal) throw new Exception("Unmatched decimal point at position '" + TokenCounter + "'");
                        else ParseDigits += Temp_string[TokenCounter];

                        if (TokenCounter + 1 >= Temp_string.Length) break;
                    }
                }


                //two tokens in a row and the second token being '-' means it a negative number
                //eg: "1 * - 1" means tokens  {"1", "*", "-1" } not {"1","*","-", "1"}
                if (SymbolsInARow >= 2 && Temp_queue.ElementAt(Temp_queue.Count - 1).Equals(Symbols.SUB))
                {
                    ParseDigits = Symbols.SUB + ParseDigits;

                    //because you can only remove the start of a queue, reverse it, dequeue, and reverse that
                    Temp_queue = new Queue<object>(Temp_queue.Reverse());
                    Temp_queue.Dequeue();
                    Temp_queue = new Queue<object>(Temp_queue.Reverse());
                }
                SymbolsInARow = 0;

                Temp_queue.Enqueue(double.Parse(ParseDigits));
            }
            #endregion
            //parse words
            #region words
            else if (Char.IsLetter(Temp_string[TokenCounter]))
            {
                //get the entire word
                StringBuilder sb = new StringBuilder();
                while (TokenCounter < Temp_string.Length && Char.IsLetter(Temp_string[TokenCounter]))
                    sb.Append(Temp_string[TokenCounter++]);

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
                        Temp_queue.Enqueue(word);
                        break;

                    //constants
                    case Constants.PI:
                        Temp_queue.Enqueue(Constants.PI_VALUE);
                        break;
                    case Constants.E:
                        Temp_queue.Enqueue(Constants.E_VALUE);
                        break;
                    case Constants.PHI:
                        Temp_queue.Enqueue(Constants.PHI_VALUE);
                        break;

                    default:
                        throw new Exception("Unknown function or constant: \"" + word + "\"");
                }

                SymbolsInARow = 0;
            }
            #endregion
            //parse symbols
            #region symbols
            else
            {
                switch (Temp_string[TokenCounter])
                {
                    case Symbols.LEFT_PAREN:
                        Temp_queue.Enqueue(Symbols.LEFT_PAREN);
                        continue;
                    case Symbols.RIGHT_PAREN:
                        Temp_queue.Enqueue(Symbols.RIGHT_PAREN);
                        continue;
                    case Symbols.MUL:
                        Temp_queue.Enqueue(Symbols.MUL);
                        SymbolsInARow++;
                        continue;
                    case Symbols.EXP:
                        Temp_queue.Enqueue(Symbols.EXP);
                        SymbolsInARow++;
                        continue;
                    case Symbols.DIV:
                        Temp_queue.Enqueue(Symbols.DIV);
                        SymbolsInARow++;
                        continue;
                    case Symbols.MOD:
                        Temp_queue.Enqueue(Symbols.MOD);
                        SymbolsInARow++;
                        continue;
                    case Symbols.ADD:
                        Temp_queue.Enqueue(Symbols.ADD);
                        SymbolsInARow++;
                        continue;
                    case Symbols.SUB:
                        Temp_queue.Enqueue(Symbols.SUB);
                        SymbolsInARow++;
                        continue;
                }
                if(Char.IsWhiteSpace(Temp_string[TokenCounter]))
                    continue;

                throw new Exception("Unknown symbol: '" + Temp_string[TokenCounter] + "'");
            }
            #endregion
        }


        return Temp_queue;
    }

    public static double Calculate(string str)
    {
        try
        {
            return Evaluator(ToRPN(Parser(str)));
        }
        catch (Exception ex)
        {
            throw new Exception("Error: " + ex.Message);
        }
    }
}

