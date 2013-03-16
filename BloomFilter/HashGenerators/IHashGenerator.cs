using System;

namespace BloomFilter
{
	public interface IHashGenerator<THash, TSeed>
	{
		unsafe THash GetHashCode (byte[] bytes, TSeed seed);
	}
}

