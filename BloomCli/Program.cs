using System;
using System.IO;
using BloomFilter;
using System.Xml;
using BloomFilter.HashGenerators;
using BloomFilter.Storage;

namespace BloomCli
{
	class Program
	{
		private const int HashSize = 51200000;

		static void Main (string[] args)
		{
			if (args == null || args.Length < 2)
				return;

			var pathInfo = new DirectoryInfo (args [0]);
			if (!pathInfo.Exists)
				throw new DirectoryNotFoundException ("Can't find " + pathInfo.FullName);

			var keyFile = new FileInfo (Path.Combine (pathInfo.FullName, args [1]));
			if (!keyFile.Exists)
				throw new FileNotFoundException ("Can't find " + keyFile.FullName);

			StandardBloomFilter filter = null;

			var bloomfilterFile = new FileInfo (Path.Combine (pathInfo.FullName, "BloomFilterData.dat"));
			if (bloomfilterFile.Exists) {

				var fileBytes = File.ReadAllBytes (bloomfilterFile.FullName);

				var estimatedSizeOfDataset = 5000000;
				var filterStorage = FilterStorageFactory.CreateBitArrayFromBytes(fileBytes);
				var hashGenerator = new Murmurhash32();
				filter = new StandardBloomFilter(estimatedSizeOfDataset, hashGenerator, filterStorage);
			} else {
				var errorRate = 0.005f;
				var estimatedSizeOfDataset = 5000000;
				var filterStorage = FilterStorageFactory.CreateBitArray(errorRate, estimatedSizeOfDataset);
				var hashGenerator = new Murmurhash32();
				filter = new StandardBloomFilter(estimatedSizeOfDataset, hashGenerator, filterStorage);

				TrainFilter (filter, keyFile);
				File.WriteAllBytes (bloomfilterFile.FullName, filter.GetBloomFilterBytes ());
			}

			string stringToTest = null;
			while (!string.IsNullOrWhiteSpace(stringToTest = Console.ReadLine())) {
				var stringInBytes = System.Text.Encoding.Default.GetBytes (stringToTest);
				var hasString = filter.Contains (stringInBytes);
				Console.WriteLine ("Known Word: " + hasString.ToString ());
			}
		}

		private static void TrainFilter (StandardBloomFilter filter, FileSystemInfo keyFile)
		{
			using (var reader = new StreamReader(keyFile.FullName)) {
					var linecount = 0;
					string line;
					while ((line = reader.ReadLine()) != null) {
						if (line.Length == 0) continue;

						++linecount;
						if (linecount % 10 == 0){
							Console.SetCursorPosition (0, 0);
							Console.WriteLine ("Reading key file: " + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%");
						}

						var stringInBytes = System.Text.Encoding.Default.GetBytes (line);
						filter.Add (stringInBytes);
					}
				}
			}
		}
}