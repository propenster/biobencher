using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioBencher
{
    [MemoryDiagnoser]
    public class MemoryBenchmarkerDemo
    {
        int NumberOfItems = 100000;

        [Benchmark]
        public void Method1()
        {
            for(int i = 0; i < 50; i++)
            {
                Console.WriteLine("I for method 1 >>> {0}", i);
            }
            Console.WriteLine("We are done for Method1");
        }
        [Benchmark]
        public void Method2()
        {
            for (int i = 0; i < 5000; i++)
            {
                Console.WriteLine("I for method 1 >>> {0}", i);
            }
            Console.WriteLine("We are done for Method2");
        }

        [Benchmark]
        public void Method3()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine("I for method 1 >>> {0}", i);
            }
            Console.WriteLine("We are done for Method3");
        }

        //[Benchmark]
        //public string ConcatStringsUsingStringBuilder()
        //{
        //    var sb = new StringBuilder();
        //    for (int i = 0; i < NumberOfItems; i++)
        //    {
        //        sb.Append("Hello World!" + i);
        //    }
        //    return sb.ToString();
        //}
        //[Benchmark]
        //public string ConcatStringsUsingGenericList()
        //{
        //    var list = new List<string>(NumberOfItems);
        //    for (int i = 0; i < NumberOfItems; i++)
        //    {
        //        list.Add("Hello World!" + i);
        //    }
        //    return list.ToString();
        //}
    }
}
