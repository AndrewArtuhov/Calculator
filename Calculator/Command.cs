using System;
using System.Windows.Input;

namespace CalculatorTest
{
    internal class Command<T> : ICommand
    {
        private readonly Action<T> _action;
        private readonly Predicate<T> _predicate;

        public event EventHandler CanExecuteChanged;

        public Command(Action<T> action, Predicate<T> predicate)
        {
            _predicate = predicate;
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is not T t)
                throw new NotSupportedException();

            return _predicate.Invoke(t);
        }

        public void Execute(object parameter)
        {
            if (parameter is not T t)
                throw new NotSupportedException();

            _action.Invoke(t);
        }
    }
}
