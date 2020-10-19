using System.Collections.Generic;

namespace CollectionsUtility
{
    public interface IUniqueList<T>
        : IList<T>
        , IReadOnlyList<T>
    {
        T ItemAt(int index);
    }
}
