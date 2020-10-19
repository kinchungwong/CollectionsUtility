using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollectionsUtility.Tests
{
    public class SortedIntListCombineTests
    {
        [Test]
        public void SortedIntListIntersect_TwoInputTest()
        {
            int sourceCount = 6;
            var source = new List<int>(Enumerable.Range(0, sourceCount));
            var bitmasks = _BitMasks_ChooseThree(sourceCount);
            int bitmaskCount = bitmasks.Count;
            for (int ka = 0; ka < bitmaskCount; ++ka)
            {
                uint maskA = bitmasks[ka];
                List<int> listA = _SelectByBitMask(source, maskA);
                for (int kb = ka; kb < bitmaskCount; ++kb)
                {
                    uint maskB = bitmasks[kb];
                    List<int> listB = _SelectByBitMask(source, maskB);
                    var intersectMockResult = new Mocks.SortedIntListIntersect(listA, listB);
                    uint maskIntersect = maskA & maskB;
                    List<int> intersectFromBitwise = _SelectByBitMask(source, maskIntersect);
                    Assert.AreEqual(intersectMockResult, intersectFromBitwise);
                }
            }
        }

        [Test]
        public void SortedIntListIntersect_ThreeInputTest()
        {
            int sourceCount = 9;
            var source = new List<int>(Enumerable.Range(0, sourceCount));
            var bitmasks = _BitMasks_ChooseThree(sourceCount);
            int bitmaskCount = bitmasks.Count;
            for (int ka = 0; ka < bitmaskCount; ++ka)
            {
                uint maskA = bitmasks[ka];
                List<int> listA = _SelectByBitMask(source, maskA);
                for (int kb = ka; kb < bitmaskCount; ++kb)
                {
                    uint maskB = bitmasks[kb];
                    List<int> listB = _SelectByBitMask(source, maskB);
                    for (int kc = kb; kc < bitmaskCount; ++kc)
                    {
                        uint maskC = bitmasks[kc];
                        List<int> listC = _SelectByBitMask(source, maskC);
                        var intersectMockResult = new Mocks.SortedIntListIntersect(listA, listB, listC);
                        uint maskIntersect = maskA & maskB & maskC;
                        List<int> intersectFromBitwise = _SelectByBitMask(source, maskIntersect);
                        Assert.AreEqual(intersectMockResult, intersectFromBitwise);
                    }
                }
            }
        }

        [Test]
        public void SortedIntListUnion_TwoInputTest()
        {
            int sourceCount = 6;
            var source = new List<int>(Enumerable.Range(0, sourceCount));
            var bitmasks = _BitMasks_ChooseThree(sourceCount);
            int bitmaskCount = bitmasks.Count;
            for (int ka = 0; ka < bitmaskCount; ++ka)
            {
                uint maskA = bitmasks[ka];
                List<int> listA = _SelectByBitMask(source, maskA);
                for (int kb = ka; kb < bitmaskCount; ++kb)
                {
                    uint maskB = bitmasks[kb];
                    List<int> listB = _SelectByBitMask(source, maskB);
                    var unionMockResult = new Mocks.SortedIntListUnion(listA, listB);
                    uint maskUnion = maskA | maskB;
                    List<int> unionFromBitwise = _SelectByBitMask(source, maskUnion);
                    Assert.AreEqual(unionMockResult, unionFromBitwise);
                }
            }
        }

        [Test]
        public void SortedIntListUnion_ThreeInputTest()
        {
            int sourceCount = 9;
            var source = new List<int>(Enumerable.Range(0, sourceCount));
            var bitmasks = _BitMasks_ChooseThree(sourceCount);
            int bitmaskCount = bitmasks.Count;
            for (int ka = 0; ka < bitmaskCount; ++ka)
            {
                uint maskA = bitmasks[ka];
                List<int> listA = _SelectByBitMask(source, maskA);
                for (int kb = ka; kb < bitmaskCount; ++kb)
                {
                    uint maskB = bitmasks[kb];
                    List<int> listB = _SelectByBitMask(source, maskB);
                    for (int kc = kb; kc < bitmaskCount; ++kc)
                    {
                        uint maskC = bitmasks[kc];
                        List<int> listC = _SelectByBitMask(source, maskC);
                        var unionMockResult = new Mocks.SortedIntListUnion(listA, listB, listC);
                        uint maskUnion = maskA | maskB | maskC;
                        List<int> unionFromBitwise = _SelectByBitMask(source, maskUnion);
                        Assert.AreEqual(unionMockResult, unionFromBitwise);
                    }
                }
            }
        }

        private List<uint> _BitMasks_ChooseThree(int count)
        {
            var result = new List<uint>();
            for (int ka = 0; ((ka + 1) + 1) < count; ++ka)
            {
                for (int kb = (ka + 1); (kb + 1) < count; ++kb)
                {
                    for (int kc = (kb + 1); kc < count; ++kc)
                    {
                        uint value = (1u << ka) | (1u << kb) | (1u << kc);
                        result.Add(value);
                    }
                }
            }
            return result;
        }

        private List<int> _SelectByBitMask(IReadOnlyList<int> source, uint bitmask)
        {
            int count = Math.Min(32, source.Count);
            var result = new List<int>(capacity: count);
            for (int index = 0; index < count; ++index)
            {
                if (((bitmask >> index) & 1u) != 0u)
                {
                    result.Add(source[index]);
                }
            }
            return result;
        }

        private string _ToString(IEnumerable<int> values)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            bool isFirst = true;
            foreach (int value in values)
            {
                sb.Append(isFirst ? " " : ", ");
                isFirst = false;
                sb.Append(value.ToString());
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
