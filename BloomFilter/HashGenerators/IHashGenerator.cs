namespace BloomFilter.HashGenerators
{
	public interface IHashGenerator<THash, TSeed>
	{
		unsafe THash GetHashCode (byte[] bytes, TSeed seed);
	}
}

