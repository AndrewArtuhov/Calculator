using System;
using System.Linq;

namespace CalculatorTest
{
    internal class ParserData
    {
        private const string _defaultValue = "0";
        private char[] _operationsChar = { '+', '-', '/', '*' };
        private readonly Calculator _calculator;

        public string Input { get; set; }
        public string Output { get; set; }
        public string Result { get; set; } 
        public string Delay { get; set; }
        public ParserData(Calculator calculator)
        {
            _calculator = calculator;
        }

        public bool LoadParse(string content, string inputView, string outputView,  string resultView, string delayView)
        {
            Input = inputView ?? _defaultValue;
            Output = outputView ?? "";
            Delay = delayView ?? _defaultValue;
            Result = resultView ?? _defaultValue;            
            return CheckDelete(content);          
        }

        private bool CheckDelete(string name)
        {
            switch (name)
            {
                case "CE":
                    Input = _defaultValue;
                    Result = _defaultValue;
                    return true;
                case "C":
                    Input = _defaultValue;
                    Result = _defaultValue;
                    Output = "";
                    return true;
                case "←":
                    if(Input.Length == 1 && Input == _defaultValue) return true;
                    Input = Input.Remove(Input.Length - 1);
                    return true;
                default:
                    return false;
            }
        }

        public void FillValueExpression(string content)
        {
            bool containsEqual = Output.Contains("=");

            if (content == "=")
            {
                EqualClick(containsEqual, content);
            }
            else if (_operationsChar.Contains(Convert.ToChar(content)))
            {
                OperationClick(content);
            }
            else
            {
                NumbersClick(containsEqual, content);
            }
        }

        private void NumbersClick(bool containsEqual, string content)
        {
            if (Input == _defaultValue && content != ",")
            {
                Input = "";
            }

            if (containsEqual)
            {
                Output = "";
                Input = content;
            }
            else if (Output == "0")
            {
                Output = content;
                Input = _defaultValue;
            }
            else if (content == ",")
            {
                var lastSymbol = Input[^1].ToString();
                if (decimal.TryParse(lastSymbol, out _))
                {
                    Input += content;
                }
            }
            else
                Input += content;
        }

        private void OperationClick(string content)
        {
            bool containsOperation = Output.Any(x => _operationsChar.Contains(x));

            if (Output.Contains("="))
                Output = Input + content;
            else if (containsOperation)
            {
                Output = Output.Replace(Output.First(x => _operationsChar.Contains(x)).ToString(), content);
            }
            else
            {
                Output += Input + content;
                Input = null;
            }
        }

        private void EqualClick(bool containsEqual, string content)
        {
            int sleep = 0;
            if (containsEqual)
            {
                return;
            }

            Output += Input;
            Input = _defaultValue;
            if (int.TryParse(Delay, out var number))
                sleep = number;
            var expression = GetExpression(Output, sleep);
            _calculator.QueueRequests.Enqueue(expression);
            Output += content;
            Input = null;
        }

        private Expression GetExpression(string fullExpression, int sleep = 0)
        {
            string value = null;
            Expression expression = new Expression();
            foreach (var symbol in fullExpression)
            {
                if (_operationsChar.Contains(symbol))
                {
                    expression.Operation = symbol.ToString();
                    expression.A = Getdouble(value);
                    value = null;
                    continue;
                }
                value += symbol;
            }
            expression.B = Getdouble(value);
            expression.Sleep = sleep;
            return expression;

            double Getdouble(string value)
            {
                if (double.TryParse(value, out var number))
                    return number;
                throw new NotSupportedException("Значение было либо слишком большим, либо слишком маленьким");
            }
        }
    }
}
