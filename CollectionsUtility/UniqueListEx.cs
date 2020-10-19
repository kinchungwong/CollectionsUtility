using System;
using System.Collections.Generic;

namespace CollectionsUtility
{
    /// <summary>
    /// <see cref="UniqueListEx{T}"/> is a list of unique items that supports item removal and 
    /// replacement.
    /// 
    /// <para>
    /// The list maintains the order of items in which they are added. 
    /// <br/>
    /// When each item is added for the first time, it is assigned an index, much like the item index 
    /// on a <see cref="List{T}"/>.
    /// <br/>
    /// Adding the same item is an idempotent operation; duplicated additions are ignored.
    /// </para>
    /// 
    /// <para>
    /// To look up an item by index, use <see cref="UniqueListBase{T}.ItemAt(int)"/>.
    /// <br/>
    /// To look up the index of an item, use <see cref="UniqueListBase{T}.IndexOf(T)"/>.
    /// </para>
    /// 
    /// <para>
    /// Item removal only affects the item being removed. The index values assigned to other items 
    /// will not be affected. 
    /// <br/>
    /// Removing an item will leave behind a hole. The index value associated with that item will 
    /// not be reused. Moreover, if an item is removed and then added again, its index value will 
    /// be reinstated.
    /// </para>
    /// 
    /// <para>
    /// Item replacement guarantees that the new item will take over the index value associated with 
    /// the old item.
    /// </para>
    /// 
    /// <para>
    /// Removal and replacement can be disabled by setting the respective properties 
    /// <see cref="CanReplace"/> and <see cref="CanRemove"/> to false.
    /// </para>
    /// 
    /// <para>
    /// This class internally uses a <see cref="Dictionary{TKey, TValue}"/> and its default equality 
    /// comparer to determine item equality and to detect duplication.
    /// <br/>
    /// Users should ensure that <see cref="TKey"/> implements <see cref="IEquatable{T}"/> and provides 
    /// a meaningful implementation of both <see cref="IEquatable{T}.Equals(T)"/> and 
    /// <see cref="object.GetHashCode()"/>.
    /// </para>
    /// 
    /// <para>
    /// The item indexers are only available via a cast to <see cref="IList{T}"/> or 
    /// <see cref="IReadOnlyList{T}"/>.
    /// <br/>
    /// The reverse indexer, <c>this[T item] =&gt; int</c>, is not provided because of a potential 
    /// ambiguity arising from an (<see cref="IUniqueList{T}"/> of <see cref="int"/>).
    /// </para>
    /// 
    /// <para>
    /// Related classes: 
    /// <br/>
    /// <see cref="UniqueList{T}"/> does not allow removal and replacement of items already added.
    /// <br/>
    /// <see cref="UniqueListEx{T}"/> allows removal and replacement of items already added, although 
    /// it requires more careful usage as the index range of items does not correspond to 
    /// <c>(0 &lt;= index &lt; Count)</c>.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of unique items that can be added to this class.
    /// </typeparam>
    /// 
    public class UniqueListEx<T>
        : UniqueListBase<T>
        , IUniqueListEx<T>
    {
        sealed public override bool CanReplace
        {
            get;
            set;
        }

        sealed public override bool CanRemove
        {
            get;
            set;
        }

        public override T this[int index]
        {
            get => ItemAt(index);
            set => ReplaceAt(index, value);
        }

        public UniqueListEx()
            : base()
        {
        }

        public UniqueListEx(int capacity)
            : base(capacity)
        {
        }

        public UniqueListEx(IEnumerable<T> items)
            : base(items)
        {
        }

        public UniqueListEx(UniqueListBase<T> other)
            : base(other)
        {
        }

        sealed public override bool Remove(T item)
        {
            if (!CanRemove)
            {
                throw new InvalidOperationException("Remove method is disallowed because CanRemove is false.");
            }
            if (!_lookup.TryGetValue(item, out int index))
            {
                return false;
            }
            if (!_flags[index])
            {
                return false;
            }
            _flags[index] = false;
            --Count;
            return true;
        }

        sealed public override void RemoveAt(int index)
        {
            if (!CanRemove)
            {
                throw new InvalidOperationException("RemoveAt method is disallowed because CanRemove is false.");
            }
            if (index < 0 || index >= _items.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            _flags[index] = false;
            --Count;
        }

        sealed public override void Replace(T oldItem, T newItem)
        {
            if (!CanReplace)
            {
                throw new InvalidOperationException("Replace method is disallowed because CanReplace is false.");
            }
            if (!_lookup.TryGetValue(oldItem, out int index))
            {
                Add(newItem);
            }
            if (!_flags[index])
            {
                _flags[index] = true;
                ++Count;
            }
            _items[index] = newItem;
            _lookup.Remove(oldItem);
            _lookup.Add(newItem, index);
        }

        sealed public override void ReplaceAt(int index, T newItem)
        {
            if (!CanReplace)
            {
                throw new InvalidOperationException("ReplaceAt method is disallowed because CanReplace is false.");
            }
            if (index < 0 || index >= _items.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (!_flags[index])
            {
                _flags[index] = true;
                ++Count;
            }
            T oldItem = _items[index];
            _items[index] = newItem;
            _lookup.Remove(oldItem);
            _lookup.Add(newItem, index);
        }
    }
}
