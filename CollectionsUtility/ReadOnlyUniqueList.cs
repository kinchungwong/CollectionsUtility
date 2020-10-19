using System;
using System.Collections;
using System.Collections.Generic;

namespace CollectionsUtility
{
    public class ReadOnlyUniqueList<T>
        : IReadOnlyUniqueList<T>
    {
        #region private
        private UniqueListBase<T> _target;
        #endregion

        public ReadOnlyUniqueList(UniqueListBase<T> target)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }
            _target = target;
        }

        public int Count => _target.Count;

        T IReadOnlyList<T>.this[int index] => ((IReadOnlyUniqueList<T>)_target)[index];

        public int IndexOf(T item) => _target.IndexOf(item);

        public T ItemAt(int index) => _target.ItemAt(index);

        public IEnumerator<T> GetEnumerator()
        {
            return _target.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_target).GetEnumerator();
        }
    }
}
