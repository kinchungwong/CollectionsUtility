using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CollectionsUtility
{
    /// <summary>
    /// <see cref="UniqueListBase{T}"/> serves as a base implementation class for both 
    /// <see cref="UniqueList{T}"/> and <see cref="UniqueListEx{T}"/>.
    /// 
    /// <para>
    /// This base class allows implementations to access its internal states without consistency
    /// guaranteed. For this reason, the constructors on this base class are marked 
    /// <see langword="internal"/> to discourage use outside the library.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The item type.
    /// </typeparam>
    /// 
    public abstract class UniqueListBase<T>
        : IUniqueList<T>
        , IReadOnlyUniqueList<T>
    {
        #region private
        protected List<T> _items;
        protected List<bool> _flags;
        protected Dictionary<T, int> _lookup;
        #endregion

        public abstract T this[int index]
        {
            get;
            set;
        }

        public int Count
        {
            get;
            protected set;
        }

        bool ICollection<T>.IsReadOnly => false;

        public abstract bool CanReplace
        {
            get;
            set;
        }

        public abstract bool CanRemove
        {
            get;
            set;
        }

        public IReadOnlyList<T> Items
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<T, int> Lookup
        {
            get;
            private set;
        }

        internal protected UniqueListBase()
        {
            _Allocate(capacity: null);
        }

        internal protected UniqueListBase(int capacity)
        {
            _Allocate(capacity: capacity);
        }

        internal protected UniqueListBase(IEnumerable<T> items)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            int? maybeCount = _StcTryGetInputCount(items);
            _Allocate(capacity: maybeCount);
            foreach (var item in items)
            {
                Add(item);
            }
        }

        internal protected UniqueListBase(UniqueListBase<T> other)
        {
            _items = new List<T>(other._items);
            _flags = new List<bool>(other._flags);
            _lookup = new Dictionary<T, int>(other._lookup);
            Items = _items.AsReadOnly();
            Lookup = new ReadOnlyDictionary<T, int>(_lookup);
            Count = other.Count;
        }

        private void _Allocate(int? capacity)
        {
            if (capacity.HasValue && capacity.Value >= 0)
            {
                _items = new List<T>(capacity: capacity.Value);
                _flags = new List<bool>(capacity: capacity.Value);
                _lookup = new Dictionary<T, int>(capacity: capacity.Value);
            }
            else
            {
                _items = new List<T>();
                _flags = new List<bool>();
                _lookup = new Dictionary<T, int>();
            }
            Items = _items.AsReadOnly();
            Lookup = new ReadOnlyDictionary<T, int>(_lookup);
            Count = 0;
        }

        private static int? _StcTryGetInputCount(IEnumerable<T> items)
        {
            switch (items)
            {
                case T[] arr:
                    return arr.Length;
                case IReadOnlyCollection<T> rocoll:
                    return rocoll.Count;
                case ICollection<T> coll:
                    return coll.Count;
                case null:
                    return 0;
                default:
                    return null;
            }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public int Add(T item)
        {
            if (!_lookup.TryGetValue(item, out int index))
            {
                _items.Add(item);
                _flags.Add(true);
                index = _items.Count - 1;
                _lookup.Add(item, index);
                ++Count;
            }
            else if (!_flags[index])
            {
                // reinstate a previously added but removed item.
                _flags[index] = true;
                ++Count;
            }
            return index;
        }

        public void Clear()
        {
            _Allocate(capacity: null);
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            var items = _items;
            var flags = _flags;
            for (int index = 0; index < items.Count; ++index)
            {
                if (!flags[index])
                {
                    continue;
                }
                array[arrayIndex++] = items[index];
            }
        }

        public int IndexOf(T item)
        {
            if (!_lookup.TryGetValue(item, out int index))
            {
                return -1;
            }
            if (!_flags[index])
            {
                return -1;
            }
            return index;
        }

        public T ItemAt(int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (!_flags[index])
            {
                throw new KeyNotFoundException($"Item at index {index} was removed.");
            }
            return _items[index];
        }

        public void Insert(int _, T item)
        {
            Add(item);
        }

        public abstract bool Remove(T item);

        public abstract void RemoveAt(int index);

        public abstract void Replace(T oldItem, T newItem);

        public abstract void ReplaceAt(int index, T newItem);

        public ReadOnlyUniqueList<T> AsReadOnly()
        {
            return new ReadOnlyUniqueList<T>(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            var items = _items;
            var flags = _flags;
            for (int index = 0; index < items.Count; ++index)
            {
                if (!flags[index])
                {
                    continue;
                }
                yield return items[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
