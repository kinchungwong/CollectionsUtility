using System.Collections;
using System.Collections.Generic;

namespace CollectionsUtility.Specialized
{
    using CollectionsUtility.Specialized.Internals;

    public class Histogram<TKey, TValue>
        : IHistogram<TKey, TValue>
        where TValue : struct
    {
        #region private 
        private UniqueList<TKey> _mapping;
        private List<TValue> _values;
        #endregion

        public IHistArith<TValue> HistArith { get; }

        public IEnumerable<TKey> Keys { get; }

        public IEnumerable<TValue> Values { get; }

        public int Count => _mapping.Count;

        public TValue DefaultIncrement { get; set; }

        public TValue this[TKey key] => _values[_mapping.IndexOf(key)];

        public Histogram(IHistArith<TValue> histArith)
        {
            HistArith = histArith;
            _mapping = new UniqueList<TKey>();
            _values = new List<TValue>();
            DefaultIncrement = HistArith.UnitValue;
            Keys = _mapping;
            Values = _values;
        }

        public void Clear()
        {
            _mapping.Clear();
            _values.Clear();
            DefaultIncrement = HistArith.UnitValue;
        }

        public void Add(TKey key)
        {
            Add(key, DefaultIncrement);
        }

        public void Add(KeyValuePair<TKey, TValue> keyAndAmount)
        {
            Add(keyAndAmount.Key, keyAndAmount.Value);
        }

        public void Add(TKey key, TValue amount)
        {
            int index = _mapping.Add(key);
            while (index >= _values.Count)
            {
                _values.Add(default);
            }
            _values[index] = HistArith.Add(_values[index], amount);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var indexAndKey in (IEnumerable<(int Index, TKey Item)>)_mapping)
            {
                yield return new KeyValuePair<TKey, TValue>(indexAndKey.Item, _values[indexAndKey.Index]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(TKey key)
        {
            return _mapping.IndexOf(key) >= 0;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = _mapping.IndexOf(key);
            if (index >= 0)
            {
                value = _values[index];
                return true;
            }
            value = default;
            return false;
        }
    }
}
