using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace BloomCli
{
    class Options
    {
        
        [Option('b', "bloomfilter", HelpText = "Output binary file for the Bloom filter.", Required = true)]
        public String BloomFilterFile { get; set; }

        [Option('n', "numitems", HelpText = "Estimated number of items in dataset.", Required = true)]
        public Int32 EstimatedNumberOfItems { get; set; }

        [Option('f', "falsepositive", HelpText = "False-positive error rate. Only applies when training.", Required = true)]
        public double FalsePositiveRate { get; set; }

        public enum ModeEnum
        {
            Train,
            Test
        }

        [Option('m', "mode", HelpText = "Train or Test", DefaultValue = ModeEnum.Train)]
        public ModeEnum Mode { get; set; }
        
        [Option('i', "input", HelpText = "Input file where each line is a key.")]
        public String InputFile { get; set; }

        [Option("merge", HelpText = "Merge input with existing Bloom filter. Only applies when training.")]
        public Boolean MergeBloomFilter { get; set; }

        [Option('v', "verbose", HelpText = "Verbose output")]
        public Boolean Verbose { get; set; }



        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText("Bloom Filter Creator by NVect");
            help.AddPreOptionsLine("------------------------------------------------");
            help.AdditionalNewLineAfterOption = true;
            help.AddOptions(this);
            try
            {
                var errors = help.RenderParsingErrorsText(this, 1);
                if (!String.IsNullOrEmpty(errors))
                {
                    help.AddPostOptionsLine(errors);
                }
            }
            catch (Exception)
            {
                //ignore
            }
            return help.ToString();

        }
    }
}
