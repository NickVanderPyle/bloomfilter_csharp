using System;

namespace BloomFilter
{
	public class BitArray2
	{
		private const int bitsPerInt = sizeof(uint) * 8;
		private const uint oneBit = 1;

		private readonly uint[] bitArray;
		public readonly int Size;
		
		public BitArray2(int size)
		{
			int dataSize = size / bitsPerInt + 1;
			this.bitArray = new uint[dataSize];
			this.Size = size;
		}

		public BitArray2(byte[] bytes)
		{
			var uintFileSize = bytes.Length / sizeof(uint);
			this.bitArray = new uint[uintFileSize];
			Buffer.BlockCopy(bytes, 0, bitArray, 0, bytes.Length);

			this.Size = this.bitArray.Length * bitsPerInt;
		}
		
		public bool this[int bitIndex]
		{
			set
			{
				var idx = bitIndex / bitsPerInt;
				var shiftCount = idx % bitsPerInt;
				if(value)
					this.bitArray[idx] |= oneBit << shiftCount;
				else
					this.bitArray[idx] &= ~(oneBit << idx % shiftCount);
			}
			get
			{
				var idx = bitIndex / bitsPerInt;
				var shiftCount = idx % bitsPerInt;
				return (this.bitArray[idx] & (oneBit << shiftCount)) == 0;
			}
		}

		public byte[] GetBytes(){
			byte[] result = new byte[this.bitArray.Length * sizeof(uint)];
			Buffer.BlockCopy(this.bitArray, 0, result, 0, result.Length);
			return result;
		}

	}

}

