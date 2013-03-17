using System;
using NUnit.Framework;
using BloomFilter;

namespace UnitTests
{
	[TestFixture()]
	public class BloomFilterTest
	{
		[TestCase(0, 0)]
		public void Filter_GivenItems_ProbablyContainsItems()
		{

		}

		public StandardBloomFilter MakeBloomFilter()
		{
			return null;
			//return new StandardBloomFilter();
		}

	}
}

