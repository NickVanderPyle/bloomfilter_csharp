using System;
using BloomFilter.HashGenerators;
using BloomFilter.Storage;

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
		    this._numberOfHashes = GetOptimalNumberOfHashes(estimatedNumberOfElements, filterStorage.Size);
		}

        public void Add(byte[] item)
        {
			for (UInt32 i = 0; i < this._numberOfHashes; ++i)
            {
				var hash = this._hashGenerator.GetHashCode(item, i);
				var index = (UInt32)(hash % this._filterBits.Size);
				this._filterBits[index] = true;
            }
        }

        public bool Contains(byte[] item)
        {
			for (UInt32 i = 0; i < this._numberOfHashes; ++i)
            {
				var hash = this._hashGenerator.GetHashCode(item, i);
				var index = (UInt32)(hash % this._filterBits.Size);
				if (!this._filterBits[index])
                {
                    return false;
                }
            }

            return true;
        }

        public byte[] GetBloomFilterBytes()
        {
            return this._filterBits.GetBytes();
        }

        #region Bloom Filter Tuning
        //Found in the book MapReduce Design Patterns

        private static int GetOptimalNumberOfHashes(int numberOfElements, int filterSize)
        {
           
            return (int)Math.Round(filterSize * Math.Log(2, Math.E) / numberOfElements);
        }

        #endregion
    }
}
