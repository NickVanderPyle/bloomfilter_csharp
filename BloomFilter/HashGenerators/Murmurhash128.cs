//https://smhasher.googlecode.com/svn/trunk/MurmurHash3.cpp
using System;
using System.Runtime.CompilerServices;

namespace BloomFilter.HashGenerators
{
	public struct HashResult128 : IEquatable<HashResult128> {
		public readonly UInt64 High;
		public readonly UInt64 Low;

		public HashResult128(UInt64 high, UInt64 low){
			this.High = high;
			this.Low = low;
		}
		
		#region IEquatable implementation
		public bool Equals (HashResult128 other)
		{
			return this.High == other.High && this.Low == other.Low;
		}
		#endregion

		public override int GetHashCode ()
		{
			return this.High.GetHashCode() ^ this.Low.GetHashCode();
		}
	}

	public class Murmurhash128 : IHashGenerator<HashResult128, UInt32>
    {

		unsafe public HashResult128 GetHashCode (byte[] bytes, UInt32 seed)
		{
			UInt64 h1 = seed;
			UInt64 h2 = seed;
			
			
			//----------
			// body
			fixed (byte* data = bytes) {
				UInt64* intPtr = (UInt64*)data;
				Int64 nblocks = bytes.LongLength / 16;

				const UInt64 c1 = 0x87c37b91114253d5;
				const UInt64 c2 = 0x4cf5ad432745937f;

				for (var i = -nblocks; i != 0; ++i)
				{
					UInt64 k1 = *intPtr++;
					UInt64 k2 = *intPtr++;

					k1 *= c1;
					k1 = Rotl64 (k1, 31);
					k1 *= c2;
					
					h1 ^= k1;
					h1 = Rotl64 (h1, 27);
					h1 += h2;
					h1 = h1 * 5 + 0x52dce729;

					k2 *= c2;
					k2  = Rotl64 (k2, 33);
					k2 *= c1;

					h2 ^= k2;
					h2 = Rotl64 (h2, 31);
					h2 += h1;
					h2 = h2 * 5 + 0x38495ab5;
				}
				
				//----------
				// tail

				byte* tail = data + nblocks * 16;
				
				UInt64 k1_tail = 0;
				UInt64 k2_tail = 0;

				switch (bytes.Length & 15) {
					case 15:
						k2_tail ^= (UInt64)tail [14] << 48;
						goto  case 14;
					case 14:
						k2_tail ^= (UInt64)tail [13] << 40;
						goto  case 13;
					case 13:
						k2_tail ^= (UInt64)tail [12] << 32;
						goto  case 12;
					case 12:
						k2_tail ^= (UInt64)tail [11] << 24;
						goto  case 11;
					case 11:
						k2_tail ^= (UInt64)tail [10] << 16;
						goto  case 10;
					case 10:
						k2_tail ^= (UInt64)tail [9] << 8;
						goto  case 9;
					case 9:
						k2_tail ^= (UInt64)tail [8];
						k2_tail *= c2;
						k2_tail  = Rotl64(k2_tail, 33);
						k2_tail *= c1;
						h2 ^= k2_tail;
						goto  case 8;
					case 8:
						k1_tail ^= (UInt64)tail [7] << 56;
						goto  case 7;
					case 7:
						k1_tail ^= (UInt64)tail [6] << 48;
						goto  case 6;
					case 6:
						k1_tail ^= (UInt64)tail [5] << 40;
						goto  case 5;
					case 5:
						k1_tail ^= (UInt64)tail [4] << 32;
						goto  case 4;
					case 4:
						k1_tail ^= (UInt64)tail [3] << 24;
						goto  case 3;
					case 3:
						k1_tail ^= (UInt64)tail [2] << 16;
						goto  case 2;
					case 2:
						k1_tail ^= (UInt64)tail [1] << 8;
						goto case 1;
					case 1:
						k1_tail ^= tail [0];
						k1_tail *= c1;
						k1_tail = Rotl64 (k1_tail, 31);
						k1_tail *= c2;
						h1 ^= k1_tail;
						break;
				}
				
				//----------
				// finalization

				h1 ^= (UInt64)bytes.LongLength;
				h2 ^= (UInt64)bytes.LongLength;

				h1 += h2;
				h2 += h1;

				h1 = fmix64 (h1);
				h2 = fmix64 (h2);

				h1 += h2;
				h2 += h1;
			}

			return new HashResult128(h1, h2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static UInt64 Rotl64(UInt64 x, int r)
		{
			return (x << r) | (x >> (64 - r));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static UInt64 fmix64(UInt64 k)
		{
			// avalanche bits
			
			k ^= k >> 33;
			k *= 0xff51afd7ed558ccdL;
			k ^= k >> 33;
			k *= 0xc4ceb9fe1a85ec53L;
			k ^= k >> 33;
			return k;
		}
    }
}
