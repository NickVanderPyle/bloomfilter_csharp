using System;

namespace BloomFilter.HashGenerators
{
	public class Murmurhash32 : IHashGenerator<UInt32, UInt32>
	{

		unsafe public UInt32 GetHashCode (byte[] bytes, UInt32 seed)
		{
			UInt32 h1 = seed;
			
			
			//----------
			// body

			fixed (byte* data = bytes) {

				int nblocks = bytes.Length / 4;
			
				const UInt32 c1 = 0xcc9e2d51;
				const UInt32 c2 = 0x1b873593;

				for (var i = -nblocks; i != 0; ++i) {
					UInt32 k1 = *((UInt32*)data);
					*data += sizeof(UInt32);
				
					k1 *= c1;
					k1 = Rotl32 (k1, 15);
					k1 *= c2;
				
					h1 ^= k1;
					h1 = Rotl32 (h1, 13); 
					h1 = h1 * 5 + 0xe6546b64;
				}
			
				//----------
				// tail
			
				byte* tail = data + nblocks * 4;
			
				UInt32 k1_tail = 0;
			
				switch (bytes.Length & 3) {
				case 3:
					k1_tail ^= (UInt32)tail [2] << 16;
					goto  case 2;
				case 2:
					k1_tail ^= (UInt32)tail [1] << 8;
					goto case 1;
				case 1:
					k1_tail ^= tail [0];
					k1_tail *= c1;
					k1_tail = Rotl32 (k1_tail, 15);
					k1_tail *= c2;
					h1 ^= k1_tail;
					break;
				}
			
				//----------
				// finalization
			
				h1 ^= (UInt32)bytes.Length;
			
				h1 = fmix32 (h1);
			}
			
			return h1;
		} 

		private static UInt32 Rotl32 ( UInt32 x, byte r )
		{
			return (x << r) | (x >> (32 - r));
		}

		private static UInt32 fmix32 ( UInt32 h )
		{
			h ^= h >> 16;
			h *= 0x85ebca6b;
			h ^= h >> 13;
			h *= 0xc2b2ae35;
			h ^= h >> 16;
			
			return h;
		}
	}
}