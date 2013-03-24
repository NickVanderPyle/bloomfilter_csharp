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

		[TestCase((UInt32)0, 14961230494313510588UL, 6383328099726337777UL)]
		[TestCase((UInt32)1, 5826198776959929748UL, 14360972042172078551UL)]
		[TestCase((UInt32)2, 13201455739478309879UL, 10921120723441872451UL)]
		[TestCase((UInt32)3, 18366858537736155884UL, 7450049429292945930UL)]
		[TestCase((UInt32)4, 6586930805225301676UL, 657024170983176706UL)]
		[TestCase((UInt32)5, 4214690439090310392UL, 15019709049338469220UL)]
		[TestCase((UInt32)6, 935305596065085843UL, 14600062569458789341UL)]
		[TestCase((UInt32)7, 19557564279890700UL, 12510223899898939502UL)]
		[TestCase((UInt32)8, 4345163627233164986UL, 3058993175989101499UL)]
		[TestCase((UInt32)9, 10852845770834771179UL, 15050809330892509503UL)]
		public void GetHash_GivenANumber_ReturnsResultsFromOriginalGoogleSource(UInt32 keyAndSeed, UInt64 high, UInt64 low)
		{
			var hashGenerator = MakeHashGenerator();
			
				Byte[] bytes = BitConverter.GetBytes (keyAndSeed);
				var result = hashGenerator.GetHashCode (bytes, keyAndSeed);
				
			Assert.AreEqual(high, result.High);
			Assert.AreEqual(low, result.Low);
		}

		public Murmurhash128 MakeHashGenerator()
		{
			return new Murmurhash128();
		}
	}
}