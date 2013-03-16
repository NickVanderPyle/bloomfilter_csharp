using System;
using System.Collections.Generic;
using BloomFilter.HashGenerators;
using NUnit.Framework;

namespace UnitTests.HashGenerators
{
	[TestFixture()]
	public class MurmurHash128Test
	{
		[TestCase(0, 0)]
		[TestCase(1, 1)]
		[TestCase(int.MinValue, int.MinValue)]
		[TestCase(int.MaxValue, int.MaxValue)]
		public void GetHash_GivenSameTwoNumbers_ReturnsSameHashResult(int left, int right)
		{
			var hashGenerator = MakeHashGenerator();
			var leftBytes = BitConverter.GetBytes (left);
			var rightBytes = BitConverter.GetBytes (right);

			var leftResult = hashGenerator.GetHashCode (leftBytes, 1);
			var rightResult = hashGenerator.GetHashCode (rightBytes, 1);

			Assert.AreEqual (leftResult, rightResult);
		}

		[TestCase(0, 1)]
		[TestCase(1, 2)]
		[TestCase(int.MinValue, int.MaxValue)]
		[TestCase(0, int.MaxValue)]
		public void GetHash_GivenDifferentTwoNumbers_ReturnsSameHashResult(int left, int right)
		{
			var hashGenerator = MakeHashGenerator();
			var leftBytes = BitConverter.GetBytes (left);
			var rightBytes = BitConverter.GetBytes (right);
			
			var leftResult = hashGenerator.GetHashCode (leftBytes, 1);
			var rightResult = hashGenerator.GetHashCode (rightBytes, 1);
			
			Assert.AreNotEqual (leftResult, rightResult);
		}

		[TestCase(new byte[] {3}, new byte[] {3})]
		[TestCase(new byte[] {3, 5, 7}, new byte[] {3, 5, 7})]
		[TestCase(new byte[] {3, 5, 7, 9, 11}, new byte[] {3, 5 ,7, 9, 11})]
		[TestCase(new byte[] {3, 5, 7, 9, 11, 13, 15}, new byte[] {3, 5 ,7, 9, 11, 13, 15})]
		[TestCase(new byte[] {3, 5, 7, 9, 11, 13, 15, 17, 19}, new byte[] {3, 5 ,7, 9, 11, 13, 15, 17, 19})]
		public void GetHash_GivenOddLengthBytes_ReturnsSameHashResult(byte[] leftBytes, byte[] rightBytes)
		{
			var hashGenerator = MakeHashGenerator();
			var leftResult = hashGenerator.GetHashCode (leftBytes, 1);
			var rightResult = hashGenerator.GetHashCode (rightBytes, 1);
			
			Assert.AreEqual (leftResult, rightResult);
		}

		[TestCase(0, UInt16.MaxValue)]
		public void GetHash_GivenSameNumberButDifferentSeed_NeverHasHashCollision(int start, int end)
		{
			var hashGenerator = MakeHashGenerator();
			var knownHashes = new List<HashResult128>(Int16.MaxValue);

			for(uint i = (uint)start; i < end; ++i){

				var result = hashGenerator.GetHashCode (new byte[] {2,1}, i);
				
				Assert.IsFalse (knownHashes.Contains(result));
				
				knownHashes.Add(result);
			}
		}

		[TestCase(0, UInt16.MaxValue)]
		public void GetHash_GivenRangeOfNumbers_NeverHasHashCollision(int start, int end)
		{
			var hashGenerator = MakeHashGenerator();
			var knownHashes = new List<HashResult128>(Int16.MaxValue);

			for(var i = start; i < end; ++i){
				Byte[] bytes = BitConverter.GetBytes (i);
				var result = hashGenerator.GetHashCode (bytes, 1);

				Assert.IsFalse (knownHashes.Contains(result));

				knownHashes.Add(result);
			}
		}

		public Murmurhash128 MakeHashGenerator()
		{
			return new Murmurhash128();
		}
	}
}