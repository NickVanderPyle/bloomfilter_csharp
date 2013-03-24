using System;

namespace BloomFilter
{
	public class BitArray2 : IFilterStorage
	{
		private const int BitsPerInt = sizeof(uint) * 8;
		private const uint OneBit = 1;

		private readonly uint[] _bitArray;
		private readonly int _size;
		
		public BitArray2(int size)
		{
			int dataSize = size / BitsPerInt + 1;
			this._bitArray = new uint[dataSize];
			this._size = size;
		}

		public BitArray2(byte[] bytes)
		{
			var uintFileSize = bytes.Length / sizeof(uint);
			this._bitArray = new uint[uintFileSize];
			Buffer.BlockCopy(bytes, 0, this._bitArray, 0, bytes.Length);

			this._size = this._bitArray.Length * BitsPerInt;
		}

		#region IFilterStorage implementation
		public bool this[UInt32 bitIndex]
		{
			set
			{
				var idx = bitIndex / BitsPerInt;
				int shiftCount = (int)(bitIndex % BitsPerInt);
				if(value){
					this._bitArray[idx] |= OneBit << shiftCount;
				}else{
					this._bitArray[idx] &= ~(OneBit << shiftCount);
				}
			}
			get
			{
				var idx = bitIndex / BitsPerInt;
				var shiftCount = (int)(bitIndex % BitsPerInt);
				return (this._bitArray[idx] & (OneBit << shiftCount)) != 0;
			}
		}

		public byte[] GetBytes(){
			byte[] result = new byte[this._bitArray.Length * sizeof(uint)];
			Buffer.BlockCopy(this._bitArray, 0, result, 0, result.Length);
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

