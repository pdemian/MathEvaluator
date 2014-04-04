# Math Evaluator

## Purpose
This project deals with turning a string expression to a numerical value.
Can be useful if used for basic expression evaluation.
The current code base is quite old, but a next version is already in the works.
Additionally a later later version will use my BigFloat library for high precision calculations

This version currently handles only these known operators:
* "()" - Grouping operator(s)
* "*"  - Multiply operator
* "/"  - Divide operator
* "-"  - Subtraction operator (also negation operator)
* "+"  - Addition operator
* "^"  - Exponential operator
* "%"  - Modulus operator

Next version will handle these additional operators:
* "log"   - Base 10 Logarithm
* "ln"    - Base e Logarithm
* "sqrt"  - Square root
* "nsqrt" - Nth Square root
* "sin"   - Sine
* "cos"   - Cosine
* "tan"   - Tangent
* "asin"  - Inverse Sin
* "acos"  - Inverse Cos
* "atan"  - Inverse Tan

As well as these constants:
* "pi" - Expands to the constant PI
* "e"  - Expands to the constant e

Stay tuned for the next version!

## Installation
Copy Calc.cs into your project

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
See Example.cs for an example
