# Math Evaluator

## Purpose
This project deals with turning a string expression to a numerical value.
Can be useful if used for basic expression evaluation.
It's first and foremost made to be as simple as possible, with an emphasis on speed where possible.
The code base is being actively worked on in the beta branch.


This version currently handles these operators:
* "()"    - Grouping operator
* "*"     - Multiply operator
* "/"     - Divide operator
* "-"     - Subtraction operator (also negation operator)
* "+"     - Addition operator
* "^"     - Exponential operator
* "%"     - Modulus operator

As well as these functions:
* "log"   - Base 10 Logarithm function
* "ln"    - Base e Logarithm function
* "sqrt"  - Square root function
* "nsqrt" - Nth Square root function
* "sin"   - Sine function
* "cos"   - Cosine function
* "tan"   - Tangent function
* "asin"  - Inverse Sin function
* "acos"  - Inverse Cos function
* "atan"  - Inverse Tan function
* "floor" - Flooring function
* "ceil"  - Ceiling function
* "round" - Rounding function
* "abs"   - Absolute value function
* "mod"   - Modulus function

Along with these constants
* "pi"  - The constant PI  (3.14159...)
* "e"   - The constant e   (2.71828...)
* "phi" - The constant PHI (1.61803...)

Additionally, the calculator is made to parse more natural input, such as "2pi" or "-5cos pi", although this may cause ambiguities and may not result in the correct answer. Use of brackets is recommended. 

The functions and constants are not case sensitive (eg, "log" and "LoG" are the same)

## Installation
Simply copy Calc.cs into your project.

## Usage
Calculate a string expression as simply as this
    
    Calc.Calculate( "some string expression" );
    
For example

    //calculate using a string
    Console.WriteLine(Calc.Calculate("16 / 2 * (8 - 3 * (4 - 2)) + 1"));
    
    //calculate using default arithmetic
    Console.WriteLine(16 / 2 * (8 - 3 * (4 - 2)) + 1);
    
Will print out
    
    17
    17
    
    
## Examples
    Console.WriteLine(Calc.Calculate("5cos pi"));
    Console.WriteLine(Calc.Calculate("abs -150"));
    Console.WriteLine(Calc.Calculate("911 mod 19"));
    Console.WriteLine(Calc.Calculate("log 27 / log 3"));
    
Prints out
    -5
    150
    18
    3
    
See the examples folder for more examples

## Upcoming preview
The next version is set to include variables, and a way to calculate the same input multiple times with a change in variable.
This would be great when graphing something. So, someone may write "3x + 5", a linear function, and you could graph x = [-5,5] without re-parsing "3x + 5" for each change in x.
Probably also with some speed improvements (like just switching to use a List<> rather than a Queue<>)

The version after that should handle boolean logic with
* "~"   - Logical Not operator
* "!"   - Logical Not operator
* "not" - Logical Not operator

* "and" - Logical And operator
* "&"   - Logical And operator

* "or"  - Logical Or operator
* "|"   - Logical Or operator

* "=="  - Equality operator
* ">="  - Greater than or equal to operator
* "<="  - Less than or equal to operator
* "!="  - Not equal to operator
* ">"   - Greater than operator
* "<"   - Less than operator

As well as:
* "true"  - Truth value (1)
* "false" - False value (0)

additionally, it should clean up the code enough to be able to add new operators without much hassle.
And possibly user defined functions.
And maybe one day: summations, and more programable structures