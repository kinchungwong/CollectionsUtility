using System;
using System.Collections.Generic;
using System.Text;

namespace CollectionsUtility
{
    public interface IUniqueListEx<T>
        : IUniqueList<T>
    {
        bool CanReplace { get; set; }

        bool CanRemove { get; set; }
    }
}
