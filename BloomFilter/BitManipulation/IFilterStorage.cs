using System;

namespace BloomFilter
{
	public interface IFilterStorage
	{
		int Size { get; }

		bool this [UInt32 index] { get; set; }

		byte[] GetBytes ();
	}
}