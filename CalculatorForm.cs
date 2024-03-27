namespace Calculator
{
    using System.Text.RegularExpressions;

    public partial class CalculatorForm : Form
    {
        private string equation = "";
        private string? result = null;

        public CalculatorForm()
        {
            InitializeComponent();
        }

        private void AddOperator(string symbol)
        {
            if (result != null)
            {
                equation = result;
                result = null;
            }
            AddToEquation(symbol);
        }

        private void AddOperand(string symbol)
        {
            if (result != null)
            {
                equation = "";
                result = null;
            }
            AddToEquation(symbol);
        }

        private void AddBracket(string symbol)
        {
            if (result != null)
            {
                result = null;
            }
            AddToEquation(symbol);
        }

        private void AddToEquation(string symbol)
        {
            this.equation += symbol;
            this.textboxResult.Text = this.equation;
        }

        private void ClearEquation()
        {
            this.equation = "";
            this.textboxResult.Text = "";
        }

        private string CalculateResult()
        {
            var tokens = EquationToTokens(equation.Replace('.', ','));
            tokens = EquationTokensToRPN(tokens);
            result = ResultFromRPN(tokens);
            return result;
        }

        private void buttonOne_Click(object sender, EventArgs e)
        {
            AddOperand("1");
        }

        private void buttonTwo_Click(object sender, EventArgs e)
        {
            AddOperand("2");
        }

        private void buttonThree_Click(object sender, EventArgs e)
        {
            AddOperand("3");
        }

        private void buttonFour_Click(object sender, EventArgs e)
        {
            AddOperand("4");
        }

        private void buttonFive_Click(object sender, EventArgs e)
        {
            AddOperand("5");
        }

        private void buttonSix_Click(object sender, EventArgs e)
        {
            AddOperand("6");
        }

        private void buttonSeven_Click(object sender, EventArgs e)
        {
            AddOperand("7");
        }

        private void buttonEight_Click(object sender, EventArgs e)
        {
            AddOperand("8");
        }

        private void buttonNine_Click(object sender, EventArgs e)
        {
            AddOperand("9");
        }

        private void buttonZero_Click(object sender, EventArgs e)
        {
            AddOperand("0");
        }

        private void buttonPlus_Click(object sender, EventArgs e)
        {
            AddOperator("+");
        }

        private void buttonMinus_Click(object sender, EventArgs e)
        {
            AddOperator("-");
        }

        private void buttonTimes_Click(object sender, EventArgs e)
        {
            AddOperator("×");
        }

        private void buttonDivision_Click(object sender, EventArgs e)
        {
            AddOperator("÷");
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            ClearEquation();
        }

        private void buttonLeftBracket_Click(object sender, EventArgs e)
        {
            AddBracket("(");
        }

        private void buttonRightBracket_Click(object sender, EventArgs e)
        {
            AddBracket(")");
        }

        private void buttonFloat_Click(object sender, EventArgs e)
        {
            AddBracket(".");
        }

        private void buttonEquals_Click(object sender, EventArgs e)
        {
            AddToEquation("=");
            AddToEquation(CalculateResult().Replace(',', '.'));
        }

        private void buttonNegate_Click(object sender, EventArgs e)
        {
            AddBracket("±");
        }

        private static List<EquationComponent> EquationToTokens(string equation)
        {
            MatchCollection matches = Regex.Matches(equation, @"([\d,±]+|\+|\-|\×|\÷)");
            List<EquationComponent> tokens = new();

            foreach (Match match in matches.Cast<Match>())
                tokens.Add(new(match.Value));

            return tokens;
        }

        private static List<EquationComponent> EquationTokensToRPN(List<EquationComponent> tokens)
        {
            List<EquationComponent> operandQueue = new();
            Stack<EquationComponent> operatorStack = new();

            foreach (EquationComponent token in tokens)
            {
                if (token.type == EquationComponent.Type.OPERAND)
                    operandQueue.Add(token);
                else if (token.type == EquationComponent.Type.LEFT_BRACKET)
                    operatorStack.Push(token);
                else if (token.type == EquationComponent.Type.OPERATOR)
                {
                    while (operatorStack.Count != 0 && operatorStack.Peek().type == EquationComponent.Type.OPERATOR && operatorStack.Peek().priority > token.priority)
                    {
                        operandQueue.Add(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
                else if (token.type == EquationComponent.Type.RIGHT_BRACKET)
                {
                    while (operatorStack.Peek().type != EquationComponent.Type.LEFT_BRACKET)
                    {
                        operandQueue.Add(operatorStack.Pop());
                    }
                    operatorStack.Pop();
                }
            }

            while (operatorStack.Count > 0)
                operandQueue.Add(operatorStack.Pop());

            return operandQueue;
        }

        private static string ResultFromRPN(List<EquationComponent> tokens)
        {
            Stack<EquationComponent> equationStack = new();

            foreach (var token in tokens)
            {
                if (token.type == EquationComponent.Type.OPERAND)
                    equationStack.Push(token);
                else
                {
                    decimal num1 = decimal.Parse(equationStack.Pop().raw);
                    decimal num2 = decimal.Parse(equationStack.Pop().raw);
                    equationStack.Push(new(EquationComponent.OperatorToFunction(token.raw)(num2, num1)));
                }
            }

            return equationStack.Pop().raw;
        }
    }

    internal class EquationComponent
    {
        private const string PLUS = "+";
        private const string MINUS = "-";
        private const string TIMES = "×";
        private const string DIVIDE = "÷";

        public enum Type
        {
            OPERATOR,
            OPERAND,
            LEFT_BRACKET,
            RIGHT_BRACKET,
        }

        public Type type;
        public int priority = 0;
        public readonly string raw;

        public EquationComponent(decimal ec) : this(ec.ToString()) { }

        public EquationComponent(string ec)
        {
            raw = ec.Replace('±', '-');

            if (raw == "(")
                type = Type.LEFT_BRACKET;
            else if (raw == ")")
                type = Type.RIGHT_BRACKET;
            else if (raw == PLUS || raw == MINUS || raw == TIMES || raw == DIVIDE)
            {
                type = Type.OPERATOR;
                priority = OperatorToPriority(raw);
            }
            else
                type = Type.OPERAND;
        }

        private static int OperatorToPriority(string o) => o switch
        {
            PLUS => 3,
            MINUS => 3,
            TIMES => 2,
            DIVIDE => 2,
            _ => throw new Exception("Invalid operator!")
        };

        public static Func<decimal, decimal, decimal> OperatorToFunction(string o) => o switch
        {
            PLUS => (x, y) => x + y,
            MINUS => (x, y) => x - y,
            TIMES => (x, y) => x * y,
            DIVIDE => (x, y) => x / y,
            _ => (x, y) => throw new Exception("Invalid operator!")
        };
    }
}
