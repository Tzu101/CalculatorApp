using System;

namespace Calculator
{
    using Operator = Func<decimal, decimal, decimal>;

    interface IMathEquation
    {
        public decimal Resolve();
        public string ToEquationString();
    }
    

    internal class EquationNumber: IMathEquation
    {
        public decimal number;

        public EquationNumber(decimal number)
        {
            this.number = number;
        }

        public decimal Resolve()
        {
            return number;
        }

        public string ToEquationString()
        {
            return number.ToString();
        }
    }

    internal class Equation: IMathEquation
    {
        public IMathEquation left;
        public string symbol;
        public IMathEquation right;

        public Equation(IMathEquation left, string symbol, IMathEquation right)
        {
            this.left = left;
            this.symbol = symbol;
            this.right = right;
        }

        public decimal Resolve()
        {
            return OperatorToFunction(symbol)(left.Resolve(), right.Resolve());
        }

        public string ToEquationString()
        {
            return left.ToString() + symbol + right.ToString();
        }

        public static Operator OperatorToFunction(string o) => o switch
        {
            "+" => (x, y) => x + y,
            "-" => (x, y) => x - y,
            "×" => (x, y) => x * y,
            "÷" => (x, y) => x / y,
            _ => (x, y) => throw new Exception("Invalid operator!")
        };
    }
}
