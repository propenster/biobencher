using CliWrap;
using CliWrap.Buffered;
using System.Diagnostics;

namespace BioBencher.Benchmark
{
    public interface IRunner
    {
        TimerResult RunCommand(int index, string command, CommandFailureAction failureAction);
    }
    public class Runner : IRunner
    {
        public string WorkingDirectory { get; }

        public Runner(string workingDirectory)
        {
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;
        }
        public TimerResult RunCommand(int index, string command, CommandFailureAction failureAction)
        {
            Console.WriteLine("Benchmark {0}: {1}", index, command);
            return Execute(command, WorkingDirectory) ?? new TimerResult();
        }

        private static string[] GetCommands(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return new string[] { };
            int firstSpaceIndex = input.IndexOf(' ');
            var firstItem = string.Empty;
            var restAsOneItem = string.Empty;
            if (firstSpaceIndex != -1)
            {
                firstItem = input.Substring(0, firstSpaceIndex);
                restAsOneItem = input.Substring(firstSpaceIndex + 1);
            }
            return new string[] { firstItem, restAsOneItem };
        }

        public async Task ExecuteAndMeasure(string commandToRun, string workingDirectory)
        {
            var timerResult = new TimerResult();
            var commandSplit = GetCommands(commandToRun);
            if (!commandSplit.Any())
            {
                //globErrors.Add("Invalid command");
                //return taskResult;
                return;
            }

            var cmd = Cli.Wrap(commandSplit.FirstOrDefault())
               .WithArguments(commandSplit.LastOrDefault())
               .WithWorkingDirectory(workingDirectory);
            Console.WriteLine("Running Task command... {0}", commandToRun);

            // Execute the command and capture the output
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var result = await cmd.ExecuteBufferedAsync();

            sw.Stop();
            timerResult.TimeReal = sw.ElapsedMilliseconds;


        }
        public TimerResult Execute(string commandToRun, string workingDirectory)
        {
            var timerResult = new TimerResult();
            var commandSplit = GetCommands(commandToRun);
            if (!commandSplit.Any())
            {
                return timerResult;
            }
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = commandSplit.FirstOrDefault(),
                Arguments = commandSplit.LastOrDefault(),
                WorkingDirectory = workingDirectory,
            };

            Process process = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true,
            };

            process.Exited += (sender, e) =>
            {
                TimeSpan totalProcessorTime = process.TotalProcessorTime;

                TimeSpan userProcessorTime = process.UserProcessorTime;
                TimeSpan kernelProcessorTime = process.PrivilegedProcessorTime;

                timerResult.TimeSystem = kernelProcessorTime.TotalMilliseconds;
                timerResult.TimeUser = userProcessorTime.TotalMilliseconds;

                timerResult.ExitStatus = process.ExitCode;


                Console.WriteLine($"Total Processor Time: {totalProcessorTime.TotalMilliseconds} ms");
                Console.WriteLine($"User Processor Time: {userProcessorTime.TotalMilliseconds} ms");
                Console.WriteLine($"Kernel Processor Time: {kernelProcessorTime.TotalMilliseconds} ms");
            };

            process.Start();
            process.WaitForExit();
            process.Dispose();


            return timerResult;

        }


    }
}
