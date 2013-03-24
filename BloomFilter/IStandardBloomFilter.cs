namespace BloomFilter
{
	public interface IStandardBloomFilter
	{
		void Add(byte[] item);
		bool Contains(byte[] item);
	}

}
