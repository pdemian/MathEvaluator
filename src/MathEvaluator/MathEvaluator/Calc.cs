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
        if (str == null || str.Count == 0) throw new Exception("empty queue");

        Stack<Object> stack = new Stack<Object>();

        for (int TokenCounter = 0; TokenCounter < str.Count; TokenCounter++)
        {
            //add numbers to the stack
            #region numbers
            if (str.ElementAt(TokenCounter) is int || str.ElementAt(TokenCounter) is double)
            {
                stack.Push(str.ElementAt(TokenCounter));
            }
            #endregion

            //calculates the stack if there is an operator in the queue
            #region operators
            if (str.ElementAt(TokenCounter) is char) 
            {
                char CurrentChar = (char)str.ElementAt(TokenCounter);

                //if we find an operator
                if (CurrentChar == Symbols.MUL ||
                    CurrentChar == Symbols.DIV ||
                    CurrentChar == Symbols.MOD ||
                    CurrentChar == Symbols.ADD ||
                    CurrentChar == Symbols.SUB ||
                    CurrentChar == Symbols.EXP)
                {
                    if (stack.Count < 2) throw new Exception("not enough tokens to evaluate the expression");


                    //do an operation dependant of the data type and 
                    double temp;
                    if (stack.Peek() is double) temp = (double)stack.Pop();
                    else                       temp = (int)stack.Pop();
                   
                    
                    if(CurrentChar == Symbols.ADD)       temp += (stack.Peek() is int ? (int)stack.Pop() : (double)stack.Pop());
                    else if (CurrentChar == Symbols.SUB) temp = (stack.Peek() is int ? (int) stack.Pop() : (double)stack.Pop()) - temp;
                    else if (CurrentChar == Symbols.MUL) temp *= (stack.Peek() is int ? (int)stack.Pop() : (double)stack.Pop());
                    else if (CurrentChar == Symbols.DIV) { if(temp == 0.0) throw new Exception("division by zero"); else temp = (stack.Peek() is int ? (int)stack.Pop() : (double)stack.Pop()) / temp; }
                    else if (CurrentChar == Symbols.MOD) { if(temp == 0.0) throw new Exception("division by zero"); else temp = (stack.Peek() is int ? (int)stack.Pop() : (double)stack.Pop()) % temp; }
                    else                                 temp = (double)Math.Pow((stack.Peek() is int ? (int)stack.Pop() : (double)stack.Pop()), temp);

                    //push the value onto the stack for the final value
                    stack.Push(temp);
                }
                else if(CurrentChar == ' ') continue;
                else throw new Exception("syntax error");
            }
            #endregion
        }

        if (stack.Count > 1) throw new Exception("too many values entered");

        return (double)stack.Pop();
    }

    //Parser converts the tokens into an RPN queue (using the Shunting Yard Algorithm)
    private static Queue<Object> Parser(Queue<Object> str)
    {
        if (str == null || str.Count == 0) throw new Exception("Empty String");

        Stack<char> Stack = new Stack<char>();
        Queue<Object> Temp_queue = new Queue<Object>();

        for (int TokenCounter = 0; TokenCounter < str.Count; TokenCounter++)
        {
            //if the token is a number, add it to the output queue.
            #region Numbers
            if (str.ElementAt(TokenCounter) is double) Temp_queue.Enqueue(str.ElementAt(TokenCounter));
            else if (str.ElementAt(TokenCounter) is int) Temp_queue.Enqueue(str.ElementAt(TokenCounter));
            #endregion

            //if the token is a left parenthesis, push it onto the stack
            #region Left Paren
            else if (str.ElementAt(TokenCounter) is char) if ((char)str.ElementAt(TokenCounter) == Symbols.LEFT_PAREN) Stack.Push(Symbols.LEFT_PAREN);
            #endregion


                else if (str.ElementAt(TokenCounter) is char)
                {
                    char CurrentChar = (char)str.ElementAt(TokenCounter);

                    //if the token is a right parenthesis, then:
                    #region Right Paren
                    if (CurrentChar == Symbols.RIGHT_PAREN)
                    {
                        bool IsParenMatched = false;

                        //Until the token at the top of the stack is a left parenthesis, pop operators off the stack onto the output queue.
                        while (Stack.Count > 0 && !IsParenMatched)
                        {
                            //Pop the left parenthesis from the stack, but not onto the output queue.
                            if (Stack.Peek() == Symbols.LEFT_PAREN)
                            {
                                Stack.Pop();
                                IsParenMatched = true;
                            }
                            else Temp_queue.Enqueue(Stack.Pop());
                        }

                        //If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
                        if (!IsParenMatched) throw new Exception("mismatched parenthesis");
                    }
                    #endregion

                    //if the token is an operator, o1, then:
                    #region Operators
                    else if (CurrentChar == Symbols.EXP ||
                             CurrentChar == Symbols.MUL ||
                             CurrentChar == Symbols.DIV ||
                             CurrentChar == Symbols.MOD ||
                             CurrentChar == Symbols.ADD ||
                             CurrentChar == Symbols.SUB)
                    {
                        //while there is an operator token, o2, at the top of the stack, and
                        //either o1 is left-associative and its precedence is less than or equal to that of o2,

                        /* HIGHEST PRECEDENCE */
                        while (Stack.Count > 0 &&
                              Stack.Peek() != Symbols.LEFT_PAREN &&
                              (CurrentChar == Symbols.MOD || CurrentChar == Symbols.MUL || CurrentChar == Symbols.DIV) &&
                              (Stack.Peek() != Symbols.ADD && Stack.Peek() != Symbols.SUB))
                        {
                            Temp_queue.Enqueue(Stack.Pop());
                        }

                        /* LOWEST PRECEDENCE */
                        while (Stack.Count > 0 &&
                            Stack.Peek() != Symbols.LEFT_PAREN &&
                            (CurrentChar == Symbols.ADD || CurrentChar == Symbols.SUB))
                        {
                            Temp_queue.Enqueue(Stack.Pop());
                        }

                        // or o1 is right-associative and its precedence is less than that of o2,

                        /* no right-associative operators other than EXP */
                        Stack.Push(CurrentChar);
                    }
                    #endregion

                }
                else throw new Exception("Unknown token: '" + str.ElementAt(TokenCounter) + "' as '" + str.ElementAt(TokenCounter).GetType() + "'");
        }
        //When there are no more tokens to read:
        //While there are still operator tokens in the stack:
        //        If the operator token on the top of the stack is a parenthesis, then there are mismatched parentheses.
        //        Pop the operator onto the output queue.
        while (Stack.Count > 0)
        {
            if (Stack.Peek() == Symbols.LEFT_PAREN) throw new Exception("mismatched parenthesis");
            Temp_queue.Enqueue(Stack.Pop());
        }


        return Temp_queue;
    }

    //Lexxer tokenizes the input string for parsing
    public static Queue<Object> Lexxer(string str)
    {
        if (str == null || str.Length == 0) throw new Exception("empty string");

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
                        else if (Temp_string[TokenCounter] == '.' && HasDecimal) throw new Exception("unmatched decimal point at position '" + TokenCounter + "'");
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

                if (HasDecimal) Temp_queue.Enqueue(double.Parse(ParseDigits));
                else Temp_queue.Enqueue(long.Parse(ParseDigits));

            }
            #endregion
            //parse words
            #region words
            else if (Char.IsLetter(Temp_string[TokenCounter]))
            {
                StringBuilder sb = new StringBuilder();
                while (TokenCounter < Temp_string.Length && Char.IsLetter(Temp_string[TokenCounter]))
                    sb.Append(Temp_string[TokenCounter++]);

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
            return Evaluator(Parser(Lexxer(str)));
        }
        catch (Exception ex)
        {
            throw new Exception("Error: " + ex.Message);
        }
    }
}

