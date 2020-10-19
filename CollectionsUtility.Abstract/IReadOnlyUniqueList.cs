using System.Collections.Generic;

namespace CollectionsUtility
{
    public interface IReadOnlyUniqueList<T>
        : IReadOnlyList<T>
    {
        int IndexOf(T item);

        T ItemAt(int index);
    }
}
