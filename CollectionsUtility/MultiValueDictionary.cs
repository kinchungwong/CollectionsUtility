using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsUtility
{
    /// <summary>
    /// <see cref="MultiValueDictionary{TKey, TValue}"/> implements an append-only, one-to-many dictionary.
    /// 
    /// <para>
    /// The abstraction exposed by this class is that of a collection of <c>KeyValuePair{TKey, TValue}</c>.
    /// <br/>
    /// The <see cref="Count"/> property refers to the number of unique <c>KeyValuePair{TKey, TValue}</c>
    /// contained in the collection.
    /// </para>
    /// 
    /// <para>
    /// When the same key is associated with multiple values, this is represented by the abstraction as 
    /// multiple instances of <c>KeyValuePair</c> having the same <c>TKey</c> but different <c>TValue</c>.
    /// </para>
    /// 
    /// <para>
    /// The collection only counts unique <c>KeyValuePair{TKey, TValue}</c>. That is, inserting the same 
    /// <c>KeyValuePair</c> more than once will not increase the size of the collection.
    /// </para>
    /// 
    /// <para>
    /// Internally, the dictionary associates each key with a <see langword="ValueTuple"/> of 
    /// <c>(TValue, <see cref="HashSet{T}"/> of TValue)</c>. When the key is associated with a single value,
    /// it is stored as-is. Creation of the <see cref="HashSet{T}"/> is deferred until a second unique value 
    /// is added for the key.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// 
    public class MultiValueDictionary<TKey, TValue>
        : ICollection<KeyValuePair<TKey, TValue>>
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
        #region private
        /// <summary>
        /// ====== Remark about valid (V, S) states ======
        /// If zero value, the dictionary should not contain an entry for that key.
        /// If one value,
        /// .... S must be null. This is used to indicate V contains a valid value.
        /// If two or more values,
        /// .... S must be non-null. This is used to indicate...
        /// .... .... S contains valid values (two or more),
        /// .... .... V does not contain a valid value.
        /// ======
        /// </summary>
        private Dictionary<TKey, (TValue V, HashSet<TValue> S)> _dict;

        private IEqualityComparer<TKey> _keyComparer;

        private IEqualityComparer<TValue> _valueComparar;
        #endregion

        public int Count { get; private set; }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        public MultiValueDictionary()
            : this(null, null)
        {
        }

        public MultiValueDictionary(IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer)
        {
            _keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _valueComparar = valueComparer ?? EqualityComparer<TValue>.Default;
            _dict = new Dictionary<TKey, (TValue V, HashSet<TValue> S)>(_keyComparer);
        }

        public void Add(KeyValuePair<TKey, TValue> kvp)
        {
            Add(kvp.Key, kvp.Value);
        }

        public void Add(TKey key, TValue value)
        {
            if (!_dict.TryGetValue(key, out var valueOrSet))
            {
                _dict.Add(key, (value, null));
                Count += 1;
                return;
            }
            if (!(valueOrSet.S is null))
            {
                if (valueOrSet.S.Add(value))
                {
                    Count += 1;
                }
                return;
            }
            if (_ValueEquals(valueOrSet.V, value))
            {
                return;
            }
            var newSet = new HashSet<TValue>();
            newSet.Add(valueOrSet.V);
            newSet.Add(value);
            if (newSet.Count != 2)
            {
                // Unexpected.
                throw new Exception();
            }
            _dict[key] = (default, newSet);
            Count += 1;
        }

        /// <summary>
        /// Retrieves at most one value associated with the key, if that exists.
        /// </summary>
        /// 
        /// <param name="key">
        /// The key.
        /// </param>
        /// 
        /// <param name="value">
        /// The outgoing parameter.
        /// </param>
        /// 
        /// <returns>
        /// False if no value is associated with the key (the key is not found in the collection).
        /// The outgoing parameter will be assigned the <see langword="default"/>.
        /// <br/>
        /// True if the key is associated with one or more values.
        /// <br/>
        /// If the key is associated with one value, it is assigned to the outgoing parameter.
        /// <br/>
        /// If the key is associated with multiple values, the outgoing parameter is assigned with 
        /// the first value obtained through the enumerator from the underlying 
        /// <see cref="HashSet{T}"/>.
        /// </returns>
        /// 
        public bool TryGetAny(TKey key, out TValue value)
        {
            if (!_dict.TryGetValue(key, out var valueOrSet))
            {
                value = default;
                return false;
            }
            if (valueOrSet.S is null)
            {
                value = valueOrSet.V;
                return true;
            }
            value = Enumerable.First(valueOrSet.S);
            return true;
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> of <see cref="TValue"/> that enumerates all 
        /// values for that key.
        /// 
        /// <para>
        /// This method does not throw any exception.
        /// </para>
        /// </summary>
        /// 
        /// <param name="key"></param>
        /// 
        /// <returns>
        /// If two or more values are associated with the key, the enumerator from the <see cref="HashSet{T}"/>
        /// is returned.
        /// <br/>
        /// Otherwise, a specialized LINQ enumerator instance is returned depending on whether 
        /// the collection contains a single value for the key or not at all.
        /// </returns>
        /// 
        public IEnumerable<TValue> TryGetAll(TKey key)
        {
            if (!_dict.TryGetValue(key, out var valueOrSet))
            {
                return Enumerable.Empty<TValue>();
            }
            if (valueOrSet.S is null)
            {
                return Enumerable.Repeat(valueOrSet.V, 1);
            }
            return valueOrSet.S;
        }

        /// <summary>
        /// Returns the number of unique values associated with the key.
        /// 
        /// <para>
        /// This method does not throw any exception.
        /// </para>
        /// </summary>
        /// 
        /// <param name="key"></param>
        /// 
        /// <returns>
        /// Zero if the collection does not contain any key-value pair for that key.
        /// <br/>
        /// One if the collection contains a single key-value pair for that key.
        /// <br/>
        /// Two or more if the collections contains two or more key-value pairs for that key.
        /// </returns>
        /// 
        public int TryGetCount(TKey key)
        {
            if (!_dict.TryGetValue(key, out var valueOrSet))
            {
                return 0;
            }
            if (valueOrSet.S is null)
            {
                return 1;
            }
            return valueOrSet.S.Count;
        }

        public void Clear()
        {
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!_dict.TryGetValue(item.Key, out var valueOrSet))
            {
                return false;
            }
            if (valueOrSet.S is null)
            {
                return _ValueEquals(item.Value, valueOrSet.V);
            }
            return valueOrSet.S.Contains(item.Value);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kvp in this)
            {
                array[arrayIndex++] = kvp;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var kvp in _dict)
            {
                if (kvp.Value.S is null)
                {
                    yield return new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value.V);
                }
                else
                {
                    foreach (var v in kvp.Value.S)
                    {
                        yield return new KeyValuePair<TKey, TValue>(kvp.Key, v);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool _ValueEquals(TValue v1, TValue v2)
        {
            return _valueComparar.Equals(v1, v2);
        }
    }
}
