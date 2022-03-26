using BenchmarkDotNet.Attributes;

namespace AdventOfCSharp.Benchmarking;

// Nullability is guaranteed given that the system operates as intended
#nullable disable

// Kept as internal for potential future consideration
// If BDN ever adds the ability to add custom references to the compiled benchmarks
[MemoryDiagnoser]
internal class ProblemBenchmark
{
    public int Year { get; set; }
    public int Day { get; set; }

    private Action solverPart1, solverPart2, loader;

    public ProblemBenchmark() { }

    [GlobalSetup]
    public void Setup()
    {
        var problemInfo = ProblemsIndex.Instance[Year, Day];

        var instance = problemInfo.InitializeInstance();
        solverPart1 = ProblemSolverMethodProvider.CreateNoReturnSolverDelegate(1, instance);
        solverPart2 = ProblemSolverMethodProvider.CreateNoReturnSolverDelegate(2, instance);
        loader = ProblemSolverMethodProvider.CreateLoadStateDelegate(instance);
        instance.EnsureLoadedState();
    }

    [Benchmark]
    public void Input()
    {
        loader();
    }
    [Benchmark]
    public void Part1()
    {
        solverPart1();
    }
    [Benchmark]
    public void Part2()
    {
        solverPart2();
    }
}
