using System;
using System.Collections.Generic;
using System.Collections;

namespace HS.Controls.Tree.Base
{
    public delegate void Generator<TStartWith,TEndWith>(TStartWith source, out TEndWith result);

    public class ItemGeneratedArgs<T> : EventArgs
    {
        private T _generated;
        public T Generated { get { return _generated; } }

        public ItemGeneratedArgs(T generated)
        {
            _generated = generated;
        }
    }

    public class ListGenerator<TStartWith,TEndWith> : ICollection
    {
        private List<TEndWith> _items;
        private int _count;
        private IEnumerator _source;
        private Generator<TStartWith, TEndWith> _generator;

        public ListGenerator(IEnumerator source, int count, Generator<TStartWith,TEndWith> generator) 
            : base()
        {
            _source = source;
            _generator = generator;
            if (count < 0)
            {
                _items = new List<TEndWith>();
                while (_source.MoveNext())
                {
                    TEndWith generated = _GenerateCurrent();
                    _items.Add(generated);
                    OnItemGenerated(generated);
                }
                _count = _items.Count;
            }
            else
            {
                _items = new List<TEndWith>(count);
                _count = count;
            }
        }

        private TEndWith _GenerateCurrent()
        {
            TStartWith sourceItem;
            TEndWith resultItem;
            sourceItem = (TStartWith)_source.Current;
            _generator(sourceItem, out resultItem);
            return resultItem;
        }

        public TEndWith this[int index]
        {
            get
            {
                while (index >= _items.Count)
                {
                    if (!_source.MoveNext())
                        throw new ArgumentOutOfRangeException();
                    TEndWith generated = _GenerateCurrent();
                    _items.Add(generated);
                    OnItemGenerated(generated);
                    if (_items.Count == Count)
                        OnEnumerationComplete();
                }
                return _items[index];
            }
        }

        public List<TEndWith> Fetched
        {
            get { return _items; }
        }

        public int IndexOf(TEndWith item)
        {
            return _items.IndexOf(item);
        }

        #region Events

        public event EventHandler EnumerationComplete;
        private void OnEnumerationComplete()
        {
            if (EnumerationComplete != null)
                EnumerationComplete(this, EventArgs.Empty);
        }

        public event EventHandler<ItemGeneratedArgs<TEndWith>> ItemGenerated;
        private void OnItemGenerated(TEndWith generated)
        {
            if (ItemGenerated != null)
                ItemGenerated(this, new ItemGeneratedArgs<TEndWith>(generated));
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        public int Count
        {
            get { return _count; }
        }

        public object SyncRoot
        {
            get { return null; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        #endregion
    }
}
