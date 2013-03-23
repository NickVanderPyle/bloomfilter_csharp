using System;

namespace BloomFilter
{
	public class BitArray2 : IFilterStorage
	{
		private const int bitsPerInt = sizeof(uint) * 8;
		private const uint oneBit = 1;

		private readonly uint[] bitArray;
		private readonly int _size;
		
		public BitArray2(int size)
		{
			int dataSize = size / bitsPerInt + 1;
			this.bitArray = new uint[dataSize];
			this._size = size;
		}

		public BitArray2(byte[] bytes)
		{
			var uintFileSize = bytes.Length / sizeof(uint);
			this.bitArray = new uint[uintFileSize];
			Buffer.BlockCopy(bytes, 0, bitArray, 0, bytes.Length);

			this._size = this.bitArray.Length * bitsPerInt;
		}

		#region IFilterStorage implementation
		public bool this[UInt32 bitIndex]
		{
			set
			{
				var idx = bitIndex / bitsPerInt;
				int shiftCount = (int)(idx % bitsPerInt);
				if(value){
					this.bitArray[idx] |= oneBit << shiftCount;
				}else{
					this.bitArray[idx] &= ~(oneBit << shiftCount);
				}
			}
			get
			{
				var idx = bitIndex / bitsPerInt;
				var shiftCount = (int)(idx % bitsPerInt);
				return (this.bitArray[idx] & (oneBit << shiftCount)) == 0;
			}
		}

		public byte[] GetBytes(){
			byte[] result = new byte[this.bitArray.Length * sizeof(uint)];
			Buffer.BlockCopy(this.bitArray, 0, result, 0, result.Length);
			return result;
		}

		public int Size {
			get {
				return this._size;
			}
		}
		#endregion
	}

}

