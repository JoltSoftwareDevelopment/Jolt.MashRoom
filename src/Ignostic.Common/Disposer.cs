using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ignostic
{
    public class Disposer : IDisposable
    {
        private Queue<IDisposable> _queue;

        public Disposer()
        {
            _queue = new Queue<IDisposable>();
        }

        public T Add<T>(T item) where T : IDisposable
        {
            _queue.Enqueue(item);
            return item;
        }

        public IEnumerable<T> AddRange<T>(IEnumerable<T> items) 
            where T : IDisposable 
        {
            foreach (var item in items)
            {
                Add(item);
            }
            return items;
        }

        public void DisposeAll()
        {
            while (_queue.Count > 0)
            {
                var item = _queue.Dequeue();
                item.Dispose();
            }
        }

        void IDisposable.Dispose()
        {
            DisposeAll();
        }

        public static void Dispose<T>(ref T disposable) where T : class, IDisposable
        {
            if (disposable != null)
            {
                disposable.Dispose();
                disposable = null;
            }
        }
    }
}
