using System;
using System.Collections.Generic;

namespace CollectionsUtility.Specialized
{
    public static class HistogramMethods
    {
        public static void AddRange<TKey, TBin>(
            this IHistogram<TKey, TBin> hist,
            IEnumerable<KeyValuePair<TKey, TBin>> keysAndIncrements)
            where TBin : struct
        {
            foreach (var kvp in keysAndIncrements)
            {
                hist.Add(kvp);
            }
        }

        public static void AddRange<TKey, TBin>(
            this IHistogram<TKey, TBin> hist,
            IEnumerable<TKey> keys, 
            TBin increment)
            where TBin : struct
        {
            foreach (var key in keys)
            {
                hist.Add(key, increment);
            }
        }

        public static void AddRange<TKey, TBin>(
            this IHistogram<TKey, TBin> hist,
            IEnumerable<TKey> keys)
            where TBin : struct
        {
            AddRange(hist, keys, hist.DefaultIncrement);
        }

        public static void CopyTo<TKey, TBin>(
            this IHistogram<TKey, TBin> hist,
            ICollection<KeyValuePair<TKey, TBin>> dest)
            where TBin : struct
        {
            foreach (var kvp in hist)
            {
                dest.Add(kvp);
            }
        }

        public static Dictionary<TKey, TBin> ToDictionary<TKey, TBin>(
            this IHistogram<TKey, TBin> hist)
            where TBin : struct
        {
            var dict = new Dictionary<TKey, TBin>();
            foreach (var kvp in hist)
            {
                dict.Add(kvp.Key, kvp.Value);
            }
            return dict;
        }


        public static KeyValuePair<TKey, TBin>[] ToArray<TKey, TBin>(
            this IHistogram<TKey, TBin> hist)
            where TBin : struct
        {
            int count = hist.Count;
            KeyValuePair<TKey, TBin>[] result = new KeyValuePair<TKey, TBin>[count];
            int index = 0;
            foreach (var kvp in hist)
            {
                result[index++] = kvp;
                if (index == count)
                {
                    break;
                }
            }
            return result;
        }

        public static KeyValuePair<TKey, TBin>[] ToSortedArray<TKey, TBin>(
            this IHistogram<TKey, TBin> hist)
            where TBin : struct
        {
            var arr = ToArray(hist);
            int CompareFunc(KeyValuePair<TKey, TBin> first, KeyValuePair<TKey, TBin> second)
            {
                return hist.HistArith.Compare(first.Value, second.Value);
            }
            Array.Sort(arr, CompareFunc);
            return arr;
        }

        public static void GetPeaks<TKey, TBin>(
            this IHistogram<TKey, TBin> hist,
            ICollection<KeyValuePair<TKey, TBin>> peaks)
            where TBin : struct
        {
            // ======
            // There are two implementation strategies (without putting any burden on Histogram class)
            // ======
            // Strategy 1: 
            // ... single pass; 
            // ... every histogram key and value is copied to the destination if its value compares 
            // ... greater than previous values
            // ======
            // Strategy 2:
            // ... two passes;
            // ... the first pass finds the peak value, the second pass populates the destination.
            // ======
            TBin? peakValue = null;
            foreach (var kvp in hist)
            {
                if (!peakValue.HasValue ||
                    hist.HistArith.Compare(kvp.Value, peakValue.Value) > 0)
                {
                    peakValue = kvp.Value;
                }
            }
            if (!peakValue.HasValue)
            {
                return;
            }
            foreach (var kvp in hist)
            {
                if (hist.HistArith.Compare(kvp.Value, peakValue.Value) >= 0)
                {
                    peaks.Add(kvp);
                }
            }
        }

        public static List<KeyValuePair<TKey, TBin>> GetPeaks<TKey, TBin>(
            this IHistogram<TKey, TBin> hist)
            where TBin : struct
        {
            var peaks = new List<KeyValuePair<TKey, TBin>>();
            GetPeaks(hist, peaks);
            return peaks;
        }
    }
}
