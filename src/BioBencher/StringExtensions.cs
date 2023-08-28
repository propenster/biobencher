using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioBencher
{
    public static class StringExtensions
    {
        public static string[] BashSplit(this string line)
        {
            return line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }
        public static string[] BashSplitNewLine(this string line)
        {
            return line.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
