using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CollectionsUtility.Tests
{
    public class MultiValueDictionaryTests
    {
        [Test]
        public void Ctor_NoArg_ShouldSucceed()
        {
            var mvdii = new MultiValueDictionary<int, int>();
            Assert.Pass();
        }

        [Test]
        public void Ctor_ArgComparersAreNull_ShouldSucceed()
        {
            var mvdii = new MultiValueDictionary<int, int>(null, null);
            Assert.Pass();
        }

        [Test]
        public void Ctor_ArgComparersAreNull_SmokeAdd_ShouldSucceed()
        {
            var mvdii = new MultiValueDictionary<int, int>(null, null);
            mvdii.Add(0, 100);
            mvdii.Add(1, 101);
            mvdii.Add(2, 102);
            mvdii.Add(3, 103);
            Assert.Pass();
        }

        public class BitMaskedIntComparer : IEqualityComparer<int>
        {
            private int _mask;

            public BitMaskedIntComparer(int mask)
            {
                _mask = mask;
            }

            public bool Equals(int first, int second)
            {
                return (first & _mask) == (second & _mask);
            }

            public int GetHashCode(int value)
            {
                return (value & _mask).GetHashCode();
            }
        }

        [Test]
        public void Ctor_Comparer_SmokeAdd_ShouldSucceed()
        {
            // A mask value of -2 (0xFFFFFFFE) causes the lowest bit to be ignored,
            // such as treating 0 and 1 as same; 2 and 3, and so on.
            const int maskValue = -2;
            var bmic = new BitMaskedIntComparer(maskValue);
            var mvdii = new MultiValueDictionary<int, int>(bmic, bmic);
            mvdii.Add(0, 100);
            mvdii.Add(1, 101);
            mvdii.Add(1, 201);
            mvdii.Add(2, 102);
            mvdii.Add(3, 103);
            mvdii.Add(3, 203);
            Assert.Pass();
        }

        [Test]
        public void Enumerator_Comparer()
        {
            // A mask value of -2 (0xFFFFFFFE) causes the lowest bit to be ignored,
            // such as treating 0 and 1 as same; 2 and 3, and so on.
            const int maskValue = -2;
            var bmic = new BitMaskedIntComparer(maskValue);
            var mvdii = new MultiValueDictionary<int, int>(bmic, bmic);
            mvdii.Add(0, 100);
            mvdii.Add(1, 101);
            mvdii.Add(1, 201);
            mvdii.Add(2, 102);
            mvdii.Add(3, 103);
            mvdii.Add(3, 203);
            Assert.That(mvdii.Count, Is.EqualTo(4));
            bool has_0_100 = false;
            bool has_0_201 = false;
            bool has_2_102 = false;
            bool has_2_203 = false;
            bool has_other = false;
            foreach (var kvp in mvdii)
            {
                switch (kvp)
                {
                    case { Key: 0, Value: 100 }:
                        has_0_100 = true;
                        break;
                    case { Key: 0, Value: 201 }:
                        has_0_201 = true;
                        break;
                    case { Key: 2, Value: 102 }:
                        has_2_102 = true;
                        break;
                    case { Key: 2, Value: 203 }:
                        has_2_203 = true;
                        break;
                    default:
                        has_other = true;
                        break;
                }
            }
            Assert.Multiple(() => 
            {
                Assert.That(has_0_100, Is.True);
                Assert.That(has_0_201, Is.True);
                Assert.That(has_2_102, Is.True);
                Assert.That(has_2_203, Is.True);
                Assert.That(has_other, Is.False);
            });
        }
    }
}
