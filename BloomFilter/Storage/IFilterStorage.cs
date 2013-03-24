using System;

namespace BloomFilter.Storage
{
	public interface IFilterStorage
	{
		int Size { get; }

		bool this [UInt32 index] { get; set; }

		byte[] GetBytes ();
	}
}