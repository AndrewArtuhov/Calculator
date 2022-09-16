using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CalculatorTest
{
    internal class MainVM : INotifyPropertyChanged
    { 
        private string _input;
        private string _output;
        private string _result;
        private string _delay;
        private string _requestCount;
        private string _resultCount;
        private readonly Calculator _calculator;
        private readonly ParserData _parser;
        private const string _requestText = "requests";
        private const string _resultText = "results";

        public event PropertyChangedEventHandler PropertyChanged;

        public string CountResult
        {
            get => _resultCount;
            set
            {
                _resultCount = value;
                OnPropertyChanged();
            }
        }
        public string CountRequest
        {
            get => _requestCount;
            set
            {
                _requestCount = value;
                OnPropertyChanged();
            }
        }

        public string Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                OnPropertyChanged();
            }
        }
        public string Input
        {
            get => _input;
            set
            { 
                _input = value;
                OnPropertyChanged();
            }
        }
        public string Output
        {
            get => _output;
            set
            {
                _output = value;
                OnPropertyChanged();
            }
        }
        public string Result
        {
            get => _result;
            set
            {
                _result = value;
                OnPropertyChanged();
            }
        }

        public ICommand InputCommand { get; }

        public MainVM()
        {
            InputCommand = new Command<string>(OnInput, IsInputActive);
            _calculator = new Calculator();
            _parser = new ParserData(_calculator);
            _calculator.QueueResult.ElementAdded += OnResultAdded;
            _calculator.QueueResult.ElementTrigger += OnTriggerQueue;
            _calculator.QueueRequests.ElementTrigger += OnTriggerQueue;
        }

        public void OnTriggerQueue()
        {         
            CountResult = _resultText + _calculator.QueueResult.Count();
            CountRequest = _requestText + _calculator.QueueRequests.Count();
        }

        public void OnResultAdded(double obj)
        {
            Result = obj.ToString();
        }

        private void OnInput(string content)  
        {
            try
            {
                if(!_parser.LoadParse(content, Input, Output, Result, Delay))
                {
                    _parser.FillValueExpression(content);
                };
                Input = _parser.Input;
                Output = _parser.Output;
                Result = _parser.Result;
                Delay = _parser.Delay;
            }
            catch
            {
                Input = "Ошибка!";
            }
        }

        private bool IsInputActive(string content)
        {
            return true;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
