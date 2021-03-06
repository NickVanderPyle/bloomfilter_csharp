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

		[TestCase(0, 500000)]
		public void GetHash_GivenSameNumberButDifferentSeed_NeverHasHashCollision(int start, int end)
		{
			var hashGenerator = MakeHashGenerator();
			var knownHashes = new HashSet<HashResult128>();

			for(uint i = (uint)start; i < end; ++i){

				var result = hashGenerator.GetHashCode (new byte[] {2,1}, i);
				
				Assert.IsFalse (knownHashes.Contains(result));
				
				knownHashes.Add(result);
			}
		}

		[TestCase(0, 500000)]
		public void GetHash_GivenRangeOfNumbers_NeverHasHashCollision(int start, int end)
		{
			var hashGenerator = MakeHashGenerator();
			var knownHashes = new HashSet<HashResult128>();

			for(var i = start; i < end; ++i){
				Byte[] bytes = BitConverter.GetBytes (i);
				var result = hashGenerator.GetHashCode (bytes, 1);

				Assert.IsFalse (knownHashes.Contains(result));

				knownHashes.Add(result);
			}
		}

		/*
		 * ExpectedValue is determined by running the MurmurHash from smhasher source
		 */
		[TestCase(0U, new byte[]{0}, 5048724184180415669UL, 5864299874987029891UL)]
		[TestCase(1U, new byte[] {1}, 9654155703026964764UL, 5257861517685671338UL)]
		[TestCase(999U, new byte[]{123,44,90,7, 33,250,200,1, 123,44,90,7, 33,250,200,1, 122,43,89,6, 32,249,199,0, 122,43,89,6, 32,249,199,1, 121,42,88,5, 31,248,198,255, 121,42,88,5, 31,248,198}, 11196537501850005970, 14840329762336191011)] //tests all of the tail switches
		public void GetHash_GivenANumber_ReturnsResultsFromOriginalGoogleSource(UInt32 seed, byte[] key, UInt64 expectedHighValue, UInt64 expectedLowValue)
		{
			var hashGenerator = MakeHashGenerator();

			var result = hashGenerator.GetHashCode (key, seed);
				
			Assert.AreEqual(expectedHighValue, result.High);
			Assert.AreEqual(expectedLowValue, result.Low);
		}

		public Murmurhash128 MakeHashGenerator()
		{
			return new Murmurhash128();
		}
	}
}