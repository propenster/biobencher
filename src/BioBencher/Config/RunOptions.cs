using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioBencher.Config
{
    public class RunOptions
    {
        [Value(0, Required = true, HelpText = "File path to the input file that contains commands/instructions\n to be benchmarked each on a single line\n")]
        public string FILE { get; set; } = string.Empty;
        [Option(shortName: 'd', "workding_directory", Required = true, HelpText = "Working directory")]
        public string WorkingDirectory { get; set; } = string.Empty;
        [Option(shortName:'i', "iterations", HelpText = "Number of times to run each benchmark item")]
        public uint Iterations { get; set; }
    }
}
