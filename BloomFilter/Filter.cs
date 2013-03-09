using System;
using System.Collections;
using BloomFilter.Hashing;

namespace BloomFilter
{
    public class Filter
    {
        private readonly int _numberOfHashes;
        private readonly BitArray2 _filterBits;

        public Filter(float falsePositiveRate, int estimatedNumberOfElements)
        {
            var optimalFilterSize = GetOptimalBloomFilterSize(estimatedNumberOfElements, falsePositiveRate);
            _numberOfHashes = GetOptimalNumberOfHashes(estimatedNumberOfElements, optimalFilterSize);
            Console.WriteLine("Optimal Filter Size: {0}/tOptimal Number Of Hashes: {1}", optimalFilterSize, _numberOfHashes);

            _filterBits = new BitArray2(optimalFilterSize);
        }

        public Filter(byte[] existingBloomFilter, float falsePositiveRate, int estimatedNumberOfElements)
        {
            var optimalFilterSize = GetOptimalBloomFilterSize(estimatedNumberOfElements, falsePositiveRate);
            _numberOfHashes = GetOptimalNumberOfHashes(estimatedNumberOfElements, optimalFilterSize);
            Console.WriteLine("Optimal Filter Size: {0}/tOptimal Number Of Hashes: {1}", optimalFilterSize, _numberOfHashes);

            _filterBits = new BitArray2(existingBloomFilter);
        }

        public void Add(byte[] item)
        {
            for (var i = 0; i < _numberOfHashes; ++i)
            {
                var index = GetHashCode(item, i);
                _filterBits[index] = true;
            }
        }

        public bool Contains(byte[] item)
        {
            for (var i = 0; i < _numberOfHashes; ++i)
            {
                var index = GetHashCode(item, i);
                if (!_filterBits[index])
                {
                    return false;
                }
            }

            return true;
        }

        private int GetHashCode(byte[] item, int offset)
        {
            //var hashCode = item.GetHashCode();
            //var mm3Hash = new Murmur3_x64((uint)offset);
            //mm3Hash.ComputeHash(item);
            //var lowerHash = (long)mm3Hash.HashLowerBound;
            //var upperHash = (long)mm3Hash.HashUpperBound;

			var hash = Murmurhash32.MurmurHash3_x86_32 (item, (uint)offset);

            var result = (int)(hash + (offset * hash)) % _filterBits.Size;

            return Math.Abs(result);
        }

        public byte[] GetBloomFilterBytes()
        {
            return this._filterBits.GetBytes();
        }

        #region Bloom Filter Tuning
        //Found in the book MapReduce Design Patterns

        private static int GetOptimalBloomFilterSize(int numberOfElements, float falsePositiveRate)
        {
            return (int)(-numberOfElements * Math.Log(falsePositiveRate) / Math.Pow(Math.Log(2), 2));
        }

        private static int GetOptimalNumberOfHashes(int numberOfElements, int filterSize)
        {
            return (int)Math.Round(filterSize * Math.Log(2) / numberOfElements);
        }

        #endregion
    }
}
