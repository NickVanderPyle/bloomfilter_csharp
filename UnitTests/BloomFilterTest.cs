using System;
using NUnit.Framework;
using BloomFilter;
using BloomFilter.HashGenerators;
using BloomFilter.Storage;

namespace UnitTests
{
	[TestFixture()]
	public class BloomFilterTest
	{
		[TestCase(new byte[]{0})]
		[TestCase(new byte[]{1})]
		[TestCase(new byte[]{99, 255, 30, 10})]
		[TestCase(new byte[]{99, 255, 30, 10, 99, 255, 30, 10, 99, 255, 30, 10})]
		public void Filter_GivenItems_ProbablyContainsItems(byte[] item)
		{
			var filter = MakeBloomFilter();

			filter.Add(item);

			Assert.IsTrue(filter.Contains(item));
		}

		[TestCase(new byte[]{0}, new byte[]{1})]
		[TestCase(new byte[]{1}, new byte[]{99, 255, 30, 10})]
		[TestCase(new byte[]{99, 255, 30, 10}, new byte[]{99, 255, 30, 10, 99, 255, 30, 10, 99, 255, 30, 10})]
		[TestCase(new byte[]{99, 255, 30, 10, 99, 255, 30, 10, 99, 255, 30, 10}, new byte[]{0})]
		public void Filter_GivenOneItem_DefinitelyDoNotContainAnotherItem(byte[] itemToContain, byte[] itemNotToContain)
		{
			var filter = MakeBloomFilter();
			
			filter.Add(itemToContain);
			
			Assert.IsFalse(filter.Contains(itemNotToContain));
		}

		public IStandardBloomFilter MakeBloomFilter()
		{
			var errorRate = 0.005f;
			var estimatedSizeOfDataset = 10;

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

