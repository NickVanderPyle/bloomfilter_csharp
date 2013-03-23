using System;
using System.Collections;
using BloomFilter.HashGenerators;

namespace BloomFilter
{
	public class StandardBloomFilter : IStandardBloomFilter
    {
        private readonly int _numberOfHashes;
		private readonly IFilterStorage _filterBits;
		private readonly IHashGenerator<uint, uint> _hashGenerator;

		public StandardBloomFilter (int estimatedNumberOfElements, IHashGenerator<uint, uint> hashGenerator, IFilterStorage filterStorage)
		{
			this._filterBits = filterStorage;
			this._hashGenerator = hashGenerator;
			_numberOfHashes = GetOptimalNumberOfHashes(estimatedNumberOfElements, filterStorage.Size);
		}

        public void Add(byte[] item)
        {
            for (UInt32 i = 0; i < _numberOfHashes; ++i)
            {
				var index = _hashGenerator.GetHashCode(item, i);
                _filterBits[index] = true;
            }
        }

        public bool Contains(byte[] item)
        {
            for (UInt32 i = 0; i < _numberOfHashes; ++i)
            {
				var index = _hashGenerator.GetHashCode(item, i);
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

			var hash = new Murmurhash32().GetHashCode(item, (uint)offset);

            var result = (int)(hash + (offset * hash)) % _filterBits.Size;

            return Math.Abs(result);
        }

        public byte[] GetBloomFilterBytes()
        {
            return this._filterBits.GetBytes();
        }

        #region Bloom Filter Tuning
        //Found in the book MapReduce Design Patterns

        private static int GetOptimalNumberOfHashes(int numberOfElements, int filterSize)
        {
            return (int)Math.Round(filterSize * Math.Log(2) / numberOfElements);
        }

        #endregion
    }
}
