using System.Collections;
using System.Collections.Generic;

namespace CollectionsUtility.Mocks
{
    /// <summary>
    /// <see cref="SortedIntListIntersect"/> takes any number of instances of integer collections, 
    /// where the integers within each instance are already sorted in ascending order, and produces
    /// an enumeration of the set intersection of the integers from all of the collections.
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
    public class SortedIntListIntersect
        : IEnumerable<int>
    {
        private SortedSet<int> _combined;

        public SortedIntListIntersect(params IEnumerable<int>[] lists)
            : this(lists as IEnumerable<IEnumerable<int>>)
        {
        }

        public SortedIntListIntersect(IEnumerable<IEnumerable<int>> lists)
        {
            _combined = new SortedSet<int>();
            bool isFirst = true;
            foreach (var list in lists)
            {
                if (isFirst)
                {
                    foreach (var value in list)
                    {
                        _combined.Add(value);
                    }
                    isFirst = false;
                }
                else
                { 
                    var nextCombined = new SortedSet<int>();
                    foreach (var value in list)
                    {
                        if (_combined.Contains(value))
                        {
                            nextCombined.Add(value);
                        }
                    }
                    _combined = nextCombined;
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
