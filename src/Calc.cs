using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class Calc
{
    private static class Symbols
    {
        public static char LEFT_PAREN = '(';
        public static char RIGHT_PAREN = ')';
        public static char EXP = '^';
        public static char MUL = '*';
        public static char DIV = '/';
        public static char MOD = '%';
        public static char ADD = '+';
        public static char SUB = '-';
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
    private static Queue<Object> Lexxer(string str)
    {
        if (str == null || str.Length == 0) throw new Exception("empty string");

        Queue<Object> Temp_queue = new Queue<object>();
        StringBuilder Temp_string = new StringBuilder(str);
        
        //used for negative numbers. If there were two operators in a row (eg, "1 * - 1"), the second minus is a negation sign
        int TokensInARow = 0;

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
                if (TokensInARow >= 2 && Temp_queue.ElementAt(Temp_queue.Count - 1).Equals(Symbols.SUB))
                {
                    ParseDigits = Symbols.SUB + ParseDigits;

                    //because you can only remove the start of a queue, reverse it, dequeue, and reverse that
                    Temp_queue = new Queue<object>(Temp_queue.Reverse());
                    Temp_queue.Dequeue();
                    Temp_queue = new Queue<object>(Temp_queue.Reverse());

                }
                TokensInARow = 0;

                if (HasDecimal) Temp_queue.Enqueue(double.Parse(ParseDigits));
                else Temp_queue.Enqueue(int.Parse(ParseDigits));

            }
            #endregion
            //parse symbols
            #region symbols
            else
            {
                switch (Temp_string[TokenCounter])
                {
                    case '(':
                        Temp_queue.Enqueue(Symbols.LEFT_PAREN);
                        continue;
                    case ')':
                        Temp_queue.Enqueue(Symbols.RIGHT_PAREN);
                        continue;
                    case '*':
                        Temp_queue.Enqueue(Symbols.MUL);
                        TokensInARow++;
                        continue;
                    case '^':
                        Temp_queue.Enqueue(Symbols.EXP);
                        TokensInARow++;
                        continue;
                    case '/':
                        Temp_queue.Enqueue(Symbols.DIV);
                        TokensInARow++;
                        continue;
                    case '%':
                        Temp_queue.Enqueue(Symbols.MOD);
                        TokensInARow++;
                        continue;
                    case '+':
                        Temp_queue.Enqueue(Symbols.ADD);
                        TokensInARow++;
                        continue;
                    case '-':
                        Temp_queue.Enqueue(Symbols.SUB);
                        TokensInARow++;
                        continue;
                    case ' ':
                        continue;
                    default:
                        throw new Exception("Unknown symbol: '" + Temp_string[TokenCounter] + "'");
                }
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

