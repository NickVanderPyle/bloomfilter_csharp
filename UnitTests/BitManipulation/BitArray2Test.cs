using System;
using NUnit.Framework;
using BloomFilter;
using BloomFilter.Storage;

namespace UnitTests
{
	[TestFixture()]
	public class BitArray2Test
	{

		[TestCase(0U, true)]
		[TestCase(1U, true)]
		[TestCase(10U, true)]
		[TestCase(100U, true)]
		[TestCase(1000U, true)]
		[TestCase(0U, false)]
		[TestCase(1U, false)]
		[TestCase(10U, false)]
		[TestCase(100U, false)]
		[TestCase(1000U, false)]
		public void Filter_GivenIndividualIndexes_AreSetToValue(UInt32 index, bool value)
		{
			var filter = MakeFilterStorage();
			
			filter[index] = value;
			
			Assert.AreEqual(value, filter[index]);
		}

		[TestCase(0U, 1U, true)]
		[TestCase(1U,  10U, true)]
		[TestCase(10U, 100U, true)]
		[TestCase(100U, 1000U, true)]
		[TestCase(0U, 1U, false)]
		[TestCase(1U,  10U, false)]
		[TestCase(10U, 100U, false)]
		[TestCase(100U, 1000U, false)]
		public void Filter_GivenIndexRange_AreSetToValue(UInt32 indexStart, UInt32 indexEnd, bool value)
		{
			var filter = MakeFilterStorage();

			for(var i = indexStart; i <= indexEnd; ++i){
				filter[i] = value;
			}

			for(var i = indexStart; i <= indexEnd; ++i){
				Assert.AreEqual(value, filter[i]);
			}
		}

		public IFilterStorage MakeFilterStorage(){
			return new BitArray2(1000);
		}
	}
}

