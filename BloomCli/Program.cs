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

        static void Main(string[] args)
        {

            var opts = new Options();
            var result = CommandLine.Parser.Default.ParseArguments(args, opts);
            if (!result)
            {
                Environment.Exit(1);
            }
            if (opts.Mode == Options.ModeEnum.Train && !opts.MergeBloomFilter && File.Exists(opts.BloomFilterFile))
            {
                File.Delete(opts.BloomFilterFile);
            }

            var bloomFilter = LoadBloomFilter(opts);

            //Train the filter
            if (opts.Mode == Options.ModeEnum.Train)
            {
                TrainFilter(opts, bloomFilter);
                File.WriteAllBytes(opts.BloomFilterFile, bloomFilter.GetBloomFilterBytes());
            }

            //Test the filter
            if (opts.Mode == Options.ModeEnum.Test)
            {
                TestFilter(opts, bloomFilter);
            }


        }

        private static StandardBloomFilter LoadBloomFilter(Options opts)
        {
            //Load Filter
            var bloomfilterFile = new FileInfo(opts.BloomFilterFile);
            StandardBloomFilter filter = null;
            var hashGenerator = new Murmurhash32();
            var numItems = opts.EstimatedNumberOfItems;
            var falsePositiveRate = opts.FalsePositiveRate;
            if (bloomfilterFile.Exists)
            {
                var fileBytes = File.ReadAllBytes(bloomfilterFile.FullName);
                var filterStorage = FilterStorageFactory.CreateBitArrayFromBytes(fileBytes, falsePositiveRate, numItems);
                filter = new StandardBloomFilter(numItems, hashGenerator, filterStorage);
            }
            else
            {
                var filterStorage = FilterStorageFactory.CreateBitArray(falsePositiveRate, numItems);
                filter = new StandardBloomFilter(numItems, hashGenerator, filterStorage);
            }
            return filter;
        }

        private static void TrainFilter(Options opts, StandardBloomFilter filter)
        {
            if (!String.IsNullOrEmpty(opts.InputFile))
            {
                //Train from file   
                var keyFile = new FileInfo(opts.InputFile);
                if (!keyFile.Exists)
                    throw new FileNotFoundException("Can't find " + keyFile.FullName);
                using (var reader = new StreamReader(keyFile.FullName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Length == 0) continue;
                        AddItem(filter, line);
                    }
                }
            }
            else
            {
                //Train from STDIN
                string item;
                while ((item = Console.ReadLine()) != null)
                {
                    AddItem(filter, item);
                }
            }
        }

        private static void AddItem(IStandardBloomFilter filter, string line)
        {
            var stringInBytes = System.Text.Encoding.Default.GetBytes(line);
            filter.Add(stringInBytes);
        }
        

        private static void TestFilter(Options opts, IStandardBloomFilter filter)
        {
            var itemsTested = 0;
            var itemsFound = 0;
            string item;
            if (!String.IsNullOrEmpty(opts.InputFile))
            {
                //Train from file   
                var keyFile = new FileInfo(opts.InputFile);
                if (!keyFile.Exists)
                    throw new FileNotFoundException("Can't find " + keyFile.FullName);
                using (var reader = new StreamReader(keyFile.FullName))
                {
                    while ((item = reader.ReadLine()) != null)
                    {
                        if (item.Length == 0) continue;
                        itemsTested ++;
                        if (TestItem(filter, item))
                        {
                            itemsFound ++;
                            Console.WriteLine(item);
                        }
                    }
                }
            }
            else
            {
                //Train from STDIN
                while ((item = Console.ReadLine()) != null)
                {
                    itemsTested ++;
                    if (TestItem(filter, item))
                    {
                        itemsFound++;
                        Console.WriteLine(item);
                    }
                }
            }
            Console.WriteLine("Items Tested: {0}\nItems Found:{1}", itemsTested, itemsFound);
        }

        private static Boolean TestItem(IStandardBloomFilter filter, string line)
        {
            var stringInBytes = System.Text.Encoding.Default.GetBytes(line);
            return filter.Contains(stringInBytes);
        }

    }

}