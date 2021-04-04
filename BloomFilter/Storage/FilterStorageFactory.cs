using System;

namespace BloomFilter.Storage
{
	public static class FilterStorageFactory
	{
        public static IFilterStorage CreateBitArrayFromBytes(byte[] bytes, double falsePositiveRate, int numberOfElements)
        {
            var optimalFilterSize = GetOptimalBloomFilterSize(falsePositiveRate, numberOfElements);
            return new BitArray2(bytes, optimalFilterSize);
		}

        public static IFilterStorage CreateBitArray(double falsePositiveRate, int numberOfElements)
		{
            var optimalFilterSize = GetOptimalBloomFilterSize(falsePositiveRate, numberOfElements);
			return new BitArray2(optimalFilterSize);
		}

        public static int GetOptimalBloomFilterSize(double falsePositiveRate, int numberOfElements)
		{
			return (int)(-numberOfElements * Math.Log(falsePositiveRate) / Math.Pow(Math.Log(2), 2));
		}
	}
}

