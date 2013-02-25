using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using BloomFilter;

namespace BloomCli
{
    class Program
    {
        private const int HashSize = 51200000;

        static void Main(string[] args)
        {
            if (args == null || args.Length < 2)
                return;

            var pathInfo = new DirectoryInfo(args[0]);
            if (!pathInfo.Exists)
                throw new DirectoryNotFoundException("Can't find " + pathInfo.FullName);

            var keyFile = new FileInfo(Path.Combine(pathInfo.FullName, args[1]));
            if (!keyFile.Exists)
                throw new FileNotFoundException("Can't find " + keyFile.FullName);

            Filter filter = null;

            var bloomfilterFile = new FileInfo(Path.Combine(pathInfo.FullName, "BloomFilterData.dat"));
            if (bloomfilterFile.Exists)
            {
                var fileBytes = File.ReadAllBytes(bloomfilterFile.FullName);
                filter = new Filter(fileBytes, 0.05f, 2000000);
            }
            else
            {
                filter = new Filter(0.05f, 2000000);
                TrainFilter(filter, keyFile);
                File.WriteAllBytes(bloomfilterFile.FullName, filter.GetBloomFilterBytes());
            }

            string stringToTest = null;
            while (!string.IsNullOrWhiteSpace(stringToTest = Console.ReadLine()))
            {
                var stringInBytes = System.Text.Encoding.Default.GetBytes(stringToTest);
                var hasString = filter.Contains(stringInBytes);
                Console.WriteLine("Known Word: " + hasString.ToString());
            }
        }

        private static void TrainFilter(Filter filter, FileInfo keyFile)
        {
            var pattern = new Regex(@"\b\w{4,}\b", RegexOptions.Compiled);
            using (var reader = new StreamReader(keyFile.FullName))
            {
                while (reader.Peek() >= 0)
                {
                    var match = pattern.Match(reader.ReadLine());
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine("Reading key file: " + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%");
                    while (match.Success)
                    {
                        var stringInBytes = System.Text.Encoding.Default.GetBytes(match.Value);
                        filter.Add(stringInBytes);
                        match = match.NextMatch();
                    }
                }
            }
        }
    }
}
