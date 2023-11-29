using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioBencher.Benchmark
{
    public class TimerResult
    {
        public int Index { get; set; }
        public double TimeReal { get; set; }
        public double TimeUser { get; set; }
        public double TimeSystem { get; set; }
        public int ExitStatus { get; set; }  
    }
}
