using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorTest
{
    public class ThreadSafeQueue<T>
    {
        private readonly Queue<T> _queue = new();
        private readonly object _locker = new();
    
        public event Action<T> ElementAdded;
        public event Action ElementTrigger;

        public string Count()
        {
            int count = 0;
            lock (_locker)
            {
                count = _queue.Count();
            }
            return count.ToString();
        }

        public void Enqueue(T t)
        {
            lock (_locker)
            {
                _queue.Enqueue(t);
            }
            ElementAdded?.Invoke(t);
            ElementTrigger?.Invoke();
        }
    
        public T Dequeue()
        {
            T t;
            lock (_locker)
            {
                t = _queue.Dequeue();
            }
            ElementTrigger?.Invoke();
            return t;
        }
    }
}
