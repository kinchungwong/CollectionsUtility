using System.Collections.Generic;

namespace CollectionsUtility.Specialized
{
    using CollectionsUtility.Specialized.Internals;

    /// <summary>
    /// Histogram.
    /// </summary>
    /// 
    /// <typeparam name="TKey">
    /// The key type.
    /// </typeparam>
    /// 
    /// <typeparam name="TValue">
    /// The quantity associated with each key. This quantity can be integer-valued or real-valued.
    /// </typeparam>
    /// 
    public interface IHistogram<TKey, TValue>
        : IReadOnlyDictionary<TKey, TValue>
        where TValue : struct
    {
        /// <summary>
        /// Basic arithmetics operating on the histogram bin value type.
        /// </summary>
        IHistArith<TValue> HistArith { get; }

        TValue DefaultIncrement { get; set; }

        void Add(TKey key);

        void Add(TKey key, TValue amount);

        void Add(KeyValuePair<TKey, TValue> keyAndAmount);
    }
}
