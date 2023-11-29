using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioBencher.Benchmark
{
    public class BenchmarkResult
    {
        public string Command { get; set; } = string.Empty;
        public double Mean { get; set; }
        public double StandardDev { get; set; }
        public double Median { get; set; }
        public double System { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double UserMean { get; set; }
        public double SystemMean { get; set; }
        public List<double> Times { get; set; } = new List<double>();
        public List<int> ExitCodes { get; set; } = new List<int>();
    }
}
