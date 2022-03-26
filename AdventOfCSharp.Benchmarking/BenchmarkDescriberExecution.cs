using BenchmarkDotNet.Running;

namespace AdventOfCSharp.Benchmarking;

public static class BenchmarkDescriberExecution
{
    public static void RunBenchmark<T>(string customProblemFileBaseDirectory)
    {
        var previousBaseDirectory = ProblemFiles.CustomBaseDirectory;
        ProblemFiles.SetCustomBaseDirectorySyncEnvironmentVariable(customProblemFileBaseDirectory);
        BenchmarkRunner.Run<T>();
        ProblemFiles.RestoreBaseDirectoryClearEnvironmentVariable(previousBaseDirectory);
    }
}
