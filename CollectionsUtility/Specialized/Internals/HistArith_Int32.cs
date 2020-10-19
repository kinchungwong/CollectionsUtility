namespace CollectionsUtility.Specialized.Internals
{
    public struct HistArith_Int32
        : IHistArith<int>
    {
        public int UnitValue => 1;

        public int Add(int arg0, int arg1)
        {
            return arg0 + arg1;
        }

        public int Compare(int arg0, int arg1)
        {
            return (arg0 > arg1) ? 1 : (arg0 < arg1) ? -1 : 0;
        }
    }
}
