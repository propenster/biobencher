using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioBencher.Benchmark
{
    public interface IBenchmarkRunner
    {
        BenchmarkResult RunBenchmark(int index);
    }
    public class BenchmarkRunner : IBenchmarkRunner
    {
        public BenchmarkRunner(string command, string workingDirectory, CommandFailureAction commandFailureAction, double minimumBenchmarkingTime, uint iterations)
        {
            Command = command;
            WorkingDirectory = workingDirectory;
            CommandFailureAction = commandFailureAction;
            MinimumBenchmarkingTime = minimumBenchmarkingTime;
            Iterations = iterations;
        }

        public string Command { get; set; }
        public string WorkingDirectory { get; set; }
        public CommandFailureAction CommandFailureAction { get; set; }
        public double MinimumBenchmarkingTime { get; set; }
        public uint Iterations { get; set; }
        public BenchmarkResult RunBenchmark(int index)
        {
            var success = true;
            var timesReal = new List<double>();
            var timesUser = new List<double>();
            var timesSystem = new List<double>();
            var exitCodes = new List<int>();

            var isCompleted = true;

            //first run.
            // Initial timing run
            var _runner = new Runner(WorkingDirectory);
            var runResponse = _runner.RunCommand(index, Command, CommandFailureAction);
            if (runResponse is null)
            {
                return new BenchmarkResult(); // throw error... Benchmark run failed...
            }

            var countRemaining = Iterations - 1;
            // Save the first result
            timesReal.Add(runResponse.TimeReal);
            timesUser.Add(runResponse.TimeUser);
            timesSystem.Add(runResponse.TimeSystem);
            exitCodes.Add(runResponse.ExitStatus);

            //progress bar reconfigure...

            for (int i = 0; i < countRemaining; i++)
            {
                //run the remaining...
                runResponse = _runner.RunCommand(index, Command, CommandFailureAction);
                if (runResponse is null)
                {
                    return new BenchmarkResult(); // throw error... Benchmark run failed...
                }

            }
            //progress bar finish...

            //compute stats...
            var tNum = timesReal.Count;
            var tMean = CalculateMean(timesReal);
            var tStdDev = 0d;
            if (timesReal.Any())
            {
                tStdDev = CalculateStandardDeviation(timesReal, tMean);
            }
            var tMedian = CalculateMedian(timesReal);
            var tMin = timesReal.Min();
            var tMax = timesReal.Max();

            var userMean = CalculateMean(timesUser);
            var systemMean = CalculateMean(timesSystem);


            var meanStr = string.Format("{0} {1}", tMean, "ms");
            var minStr = string.Format("{0} {1}", tMin, "ms");
            var maxStr = string.Format("{0} {1}", tMax, "ms");
            var numStr = string.Format("{0} iterations", tNum);

            var userStr = string.Format("{0} {1}", userMean, "ms");
            var systemStr = string.Format("{0} {1}", systemMean, "ms");

            //format and console out...
            if (timesReal.Any())
            {
                Console.WriteLine($"  Time ({Colorize("abs", ConsoleColor.Green, true)}):" +
                                  $"  {0,-8}        {Colorize($"[User: {userStr}]", ConsoleColor.Blue)}" +
                                  $"  {Colorize($"[System: {systemStr}]", ConsoleColor.Blue)}");
            }
            else
            {
                string stddevStr = string.Format("{0} {1}", tStdDev, "ms");

                Console.WriteLine($"  Time ({Colorize("mean", ConsoleColor.Green, true)} ± {Colorize("σ", ConsoleColor.Green)}):" +
                                  $"  {0,-8} ± {Colorize($"{stddevStr}", ConsoleColor.Green)}" +
                                  $"  {Colorize($"[User: {userStr}]", ConsoleColor.Blue)}" +
                                  $"  {Colorize($"[System: {systemStr}]", ConsoleColor.Blue)}");

                Console.WriteLine($"  Range ({Colorize("min", ConsoleColor.Cyan)} … {Colorize("max", ConsoleColor.Magenta)}):" +
                                  $"  {0,-8} … {Colorize($"{maxStr}", ConsoleColor.Magenta)}" +
                                  $"  {Colorize($"{numStr}", ConsoleColor.DarkGray)}");
            }


            return new BenchmarkResult
            {
                Command = Command,
                Mean = tMean,
                StandardDev = tStdDev,
                Median = tMedian,
                UserMean = userMean,
                SystemMean = systemMean,
                Min = tMin,
                Max = tMax,
                Times = timesReal,
                ExitCodes = exitCodes,

            };



        }

        static string Colorize(string text, ConsoleColor color, bool bold = false)
        {
            string colorCode = $"\u001b[{(bold ? "1;" : "")}{(int)color}m";
            string resetCode = "\u001b[0m";
            return $"{colorCode}{text}{resetCode}";
        }


        static double CalculateMedian(IEnumerable<double> values)
        {
            if (values == null || !values.Any())
                throw new ArgumentException("Input collection is null or empty.");

            List<double> sortedValues = values.OrderBy(x => x).ToList();

            int count = sortedValues.Count;
            int middleIndex = count / 2;

            if (count % 2 == 0)
            {
                double middleValue1 = sortedValues[middleIndex - 1];
                double middleValue2 = sortedValues[middleIndex];
                return (middleValue1 + middleValue2) / 2.0;
            }
            else
            {
                return sortedValues[middleIndex];
            }
        }


        static double CalculateMean(IEnumerable<double> values)
        {
            if (values == null || !values.Any())
                throw new ArgumentException("Input collection is null or empty.");

            return values.Sum() / values.Count();
        }

        static double CalculateStandardDeviation(IEnumerable<double> values, double mean)
        {
            if (values == null || !values.Any())
                throw new ArgumentException("Input collection is null or empty.");

            double sumOfSquares = values.Sum(value => Math.Pow(value - mean, 2));

            return Math.Sqrt(sumOfSquares / values.Count());
        }

        
    }
}
