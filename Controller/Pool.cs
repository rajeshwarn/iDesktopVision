using System;
using System.Collections.Concurrent;

namespace Controller
{
    public interface IPool<T>
    {
        T GetObject();
        void PutObject(T item);
    }

    public class Pool<T> : IPool<T>
    {
        private readonly ConcurrentBag<T> _objects;
        private readonly Func<T> _objectGenerator;

        public Pool(Func<T> objectGenerator)
        {
            if (objectGenerator == null) throw new ArgumentNullException("objectGenerator");
            _objects = new ConcurrentBag<T>();
            _objectGenerator = objectGenerator;
        }

        public virtual T GetObject()
        {
            T item;
            return _objects.TryTake(out item) ? item : _objectGenerator();
        }

        public virtual void PutObject(T item)
        {
            _objects.Add(item);
        }
    }
}
