using System.Collections;
using System.Collections.Generic;

namespace CollectionsUtility.Mocks
{
    /// <summary>
    /// <see cref="SortedIntListUnion"/> takes any number of instances of integer collections, 
    /// where the integers within each instance are already sorted in ascending order, and produces
    /// an enumeration of the set union of integers from all of the collections.
    /// 
    /// <para>
    /// Important.
    /// <br/>
    /// This implementation (under <see cref="Mocks"/> namespace) is a mock implementation with subpar 
    /// performance. For production use, refer to the implementation under the parent namespace.
    /// </para>
    /// 
    /// <para>
    /// The integer enumeration produced by this class is sorted ascendingly.
    /// </para>
    /// </summary>
    public class SortedIntListUnion
        : IEnumerable<int>
    {
        private SortedSet<int> _combined;

        public SortedIntListUnion(params IEnumerable<int>[] lists)
            : this(lists as IEnumerable<IEnumerable<int>>)
        { 
        }

        public SortedIntListUnion(IEnumerable<IEnumerable<int>> lists)
        {
            _combined = new SortedSet<int>();
            foreach (var list in lists)
            {
                foreach (var value in list)
                {
                    _combined.Add(value);
                }
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>)_combined).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_combined).GetEnumerator();
        }
    }
}
