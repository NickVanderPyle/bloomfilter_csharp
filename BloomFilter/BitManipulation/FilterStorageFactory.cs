using System;

namespace BloomFilter
{
	public static class FilterStorageFactory
	{
		public static IFilterStorage CreateBitArrayFromBytes (byte[] bytes)
		{
			return new BitArray2(bytes);
		}

		public static IFilterStorage CreateBitArray (float falsePositiveErrorRate, int estimatedSizeOfDataset)
		{
			var optimalFilterSize = GetOptimalBloomFilterSize(estimatedSizeOfDataset, falsePositiveErrorRate);
			return new BitArray2(optimalFilterSize);
		}

		private static int GetOptimalBloomFilterSize(int numberOfElements, float falsePositiveRate)
		{
			return (int)(-numberOfElements * Math.Log(falsePositiveRate) / Math.Pow(Math.Log(2), 2));
		}
	}
}

