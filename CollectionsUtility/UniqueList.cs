using System;
using System.Collections.Generic;

namespace CollectionsUtility
{
    /// <summary>
    /// <see cref="UniqueList{T}"/> is a list of unique items.
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
    public class UniqueList<T>
        : UniqueListBase<T>
    {
        sealed public override bool CanReplace
        {
            get => false;
            set => throw new NotSupportedException();
        }

        sealed public override bool CanRemove 
        {
            get => false;
            set => throw new NotSupportedException();
        }

        sealed public override T this[int index]
        {
            get => ItemAt(index);
            set => throw new NotSupportedException();
        }

        public UniqueList()
            : base()
        {
        }

        public UniqueList(int capacity)
            : base(capacity)
        {
        }

        public UniqueList(IEnumerable<T> items)
            : base(items)
        {
        }

        public UniqueList(UniqueListBase<T> other)
            : base(other)
        {
        }

        sealed public override bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        sealed public override void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        sealed public override void Replace(T oldItem, T newItem)
        {
            throw new NotSupportedException();
        }

        sealed public override void ReplaceAt(int index, T newItem)
        {
            throw new NotSupportedException();
        }
    }
}
