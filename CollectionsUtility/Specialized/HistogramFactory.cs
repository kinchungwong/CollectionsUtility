using System;

namespace CollectionsUtility.Specialized
{
    public static class HistogramFactory
    {
        public static Histogram<KeyType, int> CreateIntValuedHistogram<KeyType>()
        {
            return new Histogram<KeyType, int>(new Internals.HistArith_Int32());
        }

        public static Histogram<KeyType, double> CreateDoubleValuedHistogram<KeyType>()
        {
            return new Histogram<KeyType, double>(new Internals.HistArith_Float64());
        }
    }

    public static class HistogramFactory<KeyType, ValueType>
        where ValueType: struct
    {
        public static Histogram<KeyType, ValueType> Create()
        {
            switch (default(ValueType))
            {
                case int _:
                    return HistogramFactory.CreateIntValuedHistogram<KeyType>() as Histogram<KeyType, ValueType>;
                case double _:
                    return HistogramFactory.CreateDoubleValuedHistogram<KeyType>() as Histogram<KeyType, ValueType>;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
