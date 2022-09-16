using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CalculatorTest
{
    internal class Calculator
    {
        private readonly Mutex _mutex = new();

        public ThreadSafeQueue<Expression> QueueRequests { get; }
        public ThreadSafeQueue<double> QueueResult { get;  }

        public Calculator()
        {
            QueueRequests = new ThreadSafeQueue<Expression>();
            QueueResult = new ThreadSafeQueue<double>();
            QueueRequests.ElementAdded += async expression => await OnNewRequest(expression);
        }

        public async Task OnNewRequest(Expression expression)
        {
                    
            await Task.Run(Calculations);
            
        }

        private void Calculations()
        {   _mutex.WaitOne(); 
            var expression = QueueRequests.Dequeue();
            var sleep = expression.Sleep == 0 ? 1: expression.Sleep* 1000;
            Thread.Sleep(sleep);
            double result = 0;
            switch (expression.Operation)
            {
                case "-":
                    result = expression.A - expression.B;
                    break;
                case "+":
                    result = expression.A + expression.B;
                    break;
                case "/":
                    if (expression.B == 0)
                    {                        
                        return;
                    }
                    result = expression.A / expression.B;
                    break;
                case "*":
                    result = expression.A * expression.B;
                    break;
            }
            QueueResult.Enqueue(result);
            _mutex.ReleaseMutex();
        }
    }
}
