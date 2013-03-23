using System;
using NUnit.Framework;
using BloomFilter;
using BloomFilter.HashGenerators;

namespace UnitTests
{
	[TestFixture()]
	public class BloomFilterTest
	{
		[TestCase(0, 0)]
		public void Filter_GivenItems_ProbablyContainsItems()
		{
			var filter = MakeBloomFilter();
		}

		public IStandardBloomFilter MakeBloomFilter()
		{
			var errorRate = 0.005f;
			var estimatedSizeOfDataset = 5000000;

			var filterStorage = FilterStorageFactory.CreateBitArray(errorRate, estimatedSizeOfDataset);

			var hashGenerator = MakeHashGenerator();

			return new StandardBloomFilter(estimatedSizeOfDataset, hashGenerator, filterStorage);
		}

		IHashGenerator<UInt32, UInt32> MakeHashGenerator ()
		{
			return new Murmurhash32();
		}
	}
}

