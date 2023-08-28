using BenchmarkDotNet.Running;
using BioBencher;
using CliWrap;
using CliWrap.Buffered;
using System.Dynamic;
using System.Text;

public record CliRecord(string fileName, string[] args);
public class Program
{
    private static async Task Main(string[] args)
    {
        //Console.WriteLine("Hello, World!");
        //var summary = BenchmarkRunner.Run<MemoryBenchmarkerDemo>();
        var replicates = 3;
        var ompNumThreads = 1;
        var runStart = "date";
        var filePath = string.Format("{0}{1}", Environment.CurrentDirectory, "\\biobencherResults.csv");
        File.Create(filePath).Close();
        var hostName = await RunCommand(new CliRecord("hostname", new string[] { }));
        var memoryInfo = await RunCommandRetArray(new CliRecord("hostname", new string[] { "-m" }));

        var totalMemory = memoryInfo[1].BashSplit();
        var sysInfo = await RunCommandRetArray(new CliRecord("cat", new string[] { "/proc/cpuinfo" }));
        var vendorId = sysInfo[1].BashSplit();
        var cpuFamily = sysInfo[2].BashSplit();
        var model = sysInfo[3].BashSplit();
        var modelName = sysInfo[4].BashSplit();

        var compilerInfo = await RunCommand(new CliRecord("gcc", new string[] { "-v", "2>&1" }));
        compilerInfo = compilerInfo.Replace("\n", string.Empty); //chomp

        var compilerParse = compilerInfo.Split(new string[] { "version" }, StringSplitOptions.None);
        var compiler = compilerParse[1].BashSplit();
        var modelLength = modelName.Length - 1;

        var sliceModelName = modelName[3..modelLength];
        var speed = sysInfo[4].Split(new char[] { '@' });
        var cache = sysInfo[8].BashSplit();
        var cores = sysInfo[12].BashSplit();
        var prettySpeed = TrimSpeed(speed[1]);

        var prettyModelName = string.Format("{0} {1} {2} {3}", sliceModelName[0], sliceModelName[1], sliceModelName[2], sliceModelName[3]);
        prettyModelName = prettyModelName.Replace("\n", string.Empty); //chomp
        hostName = hostName.Replace("\n", string.Empty); //chomp

        var hwInfo = string.Format("{0},{1},{2},{3},{4},{5}KB,{6}MB,gcc-{7}", vendorId[2], cpuFamily[3], model[2], prettyModelName, prettySpeed, cache[3], cores[3], totalMemory, compiler[1]);
        Console.WriteLine("\nBeginning Bio-Bencher Benchmarking:\n");
        File.AppendAllText(filePath, string.Format("Run Start: {0}{1}", runStart, Environment.NewLine));
        File.AppendAllText(filePath, string.Format("Run Start: {0}{1}", "host,test,replicate,runTime,cpuVendor,cpuFamily,cpuModel,procSpeed,cacheSize,numCores,totalMem,compiler", Environment.NewLine));

        var timeResults = new string[] { };
        var userTime = new string[] { };
        var sysTime = new string[] { };
        var totalTime = 0L;
        var output = string.Empty;
        var toolsExecBuilder = new StringBuilder();
        Command cmd = null;
        BufferedCommandResult result = null;

        for (int i = 1; i < replicates; i++)
        {
            //BEDTools v. 2.25.0
            Console.WriteLine($"{0}Running BEDTools, replicate # {1}...{2}{3}", Environment.NewLine, i, Environment.NewLine, Environment.NewLine);
            Thread.Sleep(5000);
            //(time -p sh -c 'bedtools window -l 1000 -r 1000 -a ./inputs/BEDTools/reads.gff -b ./inputs/BEDTools/start.gff | bedtools getfasta -fi ./inputs/BEDTools/yeast.fasta -bed stdin -fo stdout 1> ./outputs/BEDTools/results.txt 2>> benchResults.txt') 2>&1
            string bedToolsBashCommand = "time -p sh -c 'bedtools window -l 1000 -r 1000 -a ./inputs/BEDTools/reads.gff -b ./inputs/BEDTools/start.gff | bedtools getfasta -fi ./inputs/BEDTools/yeast.fasta -bed stdin -fo stdout 1> ./outputs/BEDTools/results.txt 2>> benchResults.txt'";
            cmd = Cli.Wrap("bash").WithArguments($"-c \"{bedToolsBashCommand}\"")
                //.WithStandardOutputPipe(PipeTarget.ToStringBuilder(toolsExecBuilder));
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line => toolsExecBuilder.AppendLine(line.TrimEnd('\n'))));
            result = await cmd.ExecuteBufferedAsync();
            output = toolsExecBuilder.ToString() ?? string.Empty;
            timeResults = output.BashSplitNewLine();
            Console.WriteLine(string.Join(" ", timeResults));
            userTime = timeResults[1].BashSplit();
            sysTime = timeResults[2].BashSplit();
            totalTime = long.Parse(userTime[1]) + long.Parse(sysTime[1]);

            File.AppendAllText(filePath, string.Format("{0},{1},{2},{3},{4}{5}", hostName, "bedtools", i, totalTime, hwInfo, Environment.NewLine));
            toolsExecBuilder.Clear();



            // CLUSTALW2 v. 2.1
            Console.WriteLine($"{0}Running Clustalw2, replicate # {1}...{2}{3}", Environment.NewLine, i, Environment.NewLine, Environment.NewLine);
            Thread.Sleep(5000);
            //(time -p sh -c 'clustalw2 -INFILE=./inputs/clustalw/tufa420.seq -OUTPUT=PHYLIP 1> ./outputs/clustalw/results.txt 2>> benchResults.txt') 2>&1
            string clustalw2BashCommand = "(time -p sh -c 'clustalw2 -INFILE=./inputs/clustalw2/tufa420.seq -OUTPUT=PHYLIP 1> ./outputs/clustalw2/results.txt 2>> benchResults.txt') 2>&1";
            cmd = Cli.Wrap("bash").WithArguments($"-c \"{clustalw2BashCommand}\"")
                //.WithStandardOutputPipe(PipeTarget.ToStringBuilder(toolsExecBuilder));
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line => toolsExecBuilder.AppendLine(line.TrimEnd('\n'))));
            result = await cmd.ExecuteBufferedAsync();
            output = toolsExecBuilder.ToString() ?? string.Empty;
            timeResults = output.BashSplitNewLine();
            Console.WriteLine(string.Join(" ", timeResults));
            userTime = timeResults[1].BashSplit();
            sysTime = timeResults[2].BashSplit();
            totalTime = long.Parse(userTime[1]) + long.Parse(sysTime[1]);

            File.AppendAllText(filePath, string.Format("{0},{1},{2},{3},{4}{5}", hostName, "clustalw2", i, totalTime, hwInfo, Environment.NewLine));
            toolsExecBuilder.Clear();

            // BLAST 2.30.0
            Console.WriteLine($"{0}Running BLAST, replicate # {1}...{2}{3}", Environment.NewLine, i, Environment.NewLine, Environment.NewLine);
            Thread.Sleep(5000);
            //(time -p sh -c 'blast blastn -d ./inputs/BLAST/nt -i ./inputs/BLAST/batch2.fa  1> ./output/BLAST/results.txt 2>> benchResults.txt') 2>&1`;
            string blastBashCommand = "(time -p sh -c 'blast blastn -d ./inputs/BLAST/nt -i ./inputs/BLAST/batch2.fa  1> ./output/BLAST/results.txt 2>> benchResults.txt') 2>&1";
            cmd = Cli.Wrap("bash").WithArguments($"-c \"{blastBashCommand}\"")
                //.WithStandardOutputPipe(PipeTarget.ToStringBuilder(toolsExecBuilder));
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line => toolsExecBuilder.AppendLine(line.TrimEnd('\n'))));
            result = await cmd.ExecuteBufferedAsync();
            output = toolsExecBuilder.ToString() ?? string.Empty;
            timeResults = output.BashSplitNewLine();
            Console.WriteLine(string.Join(" ", timeResults));
            userTime = timeResults[1].BashSplit();
            sysTime = timeResults[2].BashSplit();
            totalTime = long.Parse(userTime[1]) + long.Parse(sysTime[1]);

            File.AppendAllText(filePath, string.Format("{0},{1},{2},{3},{4}{5}", hostName, "blast", i, totalTime, hwInfo, Environment.NewLine));
            toolsExecBuilder.Clear();

            // HMMER v.3.0
            Console.WriteLine($"{0}Running HMMER, replicate # {1}...{2}{3}", Environment.NewLine, i, Environment.NewLine, Environment.NewLine);
            Thread.Sleep(5000);
            //(time -p sh -c 'hmmsearch ./inputs/HMMER/tufa420.hmm ./inputs/HMMER/uniprot_sprot.fasta  1> ./outputs/HMMER/results.txt 2>> benchResults.txt') 2>&1
            string hmmerBashCommand = "(time -p sh -c 'hmmsearch ./inputs/HMMER/tufa420.hmm ./inputs/HMMER/uniprot_sprot.fasta  1> ./outputs/HMMER/results.txt 2>> benchResults.txt') 2>&1";
            cmd = Cli.Wrap("bash").WithArguments($"-c \"{hmmerBashCommand}\"")
                //.WithStandardOutputPipe(PipeTarget.ToStringBuilder(toolsExecBuilder));
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line => toolsExecBuilder.AppendLine(line.TrimEnd('\n'))));
            result = await cmd.ExecuteBufferedAsync();
            output = toolsExecBuilder.ToString() ?? string.Empty;
            timeResults = output.BashSplitNewLine();
            Console.WriteLine(string.Join(" ", timeResults));
            userTime = timeResults[1].BashSplit();
            sysTime = timeResults[2].BashSplit();
            totalTime = long.Parse(userTime[1]) + long.Parse(sysTime[1]);

            File.AppendAllText(filePath, string.Format("{0},{1},{2},{3},{4}{5}", hostName, "hmmer", i, totalTime, hwInfo, Environment.NewLine));
            toolsExecBuilder.Clear();


            // PHYLIP
            Console.WriteLine($"{0}Running HMMER, replicate # {1}...{2}{3}", Environment.NewLine, i, Environment.NewLine, Environment.NewLine);
            Thread.Sleep(5000);
            //(time -p sh -c 'phylip protdist ./inputs/PHYLIP/tufa420.phy 1> ./outputs/PHYLIP/results.txt 2>> benchResults.txt') 2>&1
            string phylipBashCommand = "(time -p sh -c 'phylip protdist ./inputs/PHYLIP/tufa420.phy 1> ./outputs/PHYLIP/results.txt 2>> benchResults.txt') 2>&1";
            cmd = Cli.Wrap("bash").WithArguments($"-c \"{phylipBashCommand}\"")
                //.WithStandardOutputPipe(PipeTarget.ToStringBuilder(toolsExecBuilder));
                .WithStandardOutputPipe(PipeTarget.ToDelegate(line => toolsExecBuilder.AppendLine(line.TrimEnd('\n'))));
            result = await cmd.ExecuteBufferedAsync();
            output = toolsExecBuilder.ToString() ?? string.Empty;
            timeResults = output.BashSplitNewLine();
            Console.WriteLine(string.Join(" ", timeResults));
            userTime = timeResults[1].BashSplit();
            sysTime = timeResults[2].BashSplit();
            totalTime = long.Parse(userTime[1]) + long.Parse(sysTime[1]);

            File.AppendAllText(filePath, string.Format("{0},{1},{2},{3},{4}{5}", hostName, "phylip", i, totalTime, hwInfo, Environment.NewLine));
            toolsExecBuilder.Clear();

            //add QuEST

            //add VELVET



        }

        Console.WriteLine("DONE!");
        Console.WriteLine("Benchmarking results written to: {0}", filePath);
        Console.WriteLine("Extraneous run output dumped to: benchResults.txt");
        var runEnd = await RunCommand(new CliRecord("date", new string[] { }));

        File.AppendAllText(filePath, string.Format("Run End: {0},{1}", runEnd, Environment.NewLine));


    }

    private static object TrimSpeed(string v)
    {
        string output = v.TrimStart();
        output = output.TrimEnd();
        return output;
    }

    private static async Task<string> RunCommand(CliRecord cli)
    {
        var sb = new StringBuilder();
        await Cli.Wrap(cli.fileName).WithArguments(cli.args).WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb)).ExecuteAsync();
        return sb.ToString();
    }

    private static async Task<string[]> RunCommandRetArray(CliRecord cli)
    {
        var sb = new StringBuilder();
        await Cli.Wrap(cli.fileName).WithArguments(cli.args).WithStandardOutputPipe(PipeTarget.ToStringBuilder(sb)).ExecuteAsync();
        return sb.ToString().BashSplitNewLine();
    }
}