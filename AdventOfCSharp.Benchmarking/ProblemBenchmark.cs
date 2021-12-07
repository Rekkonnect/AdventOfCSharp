using BenchmarkDotNet.Attributes;

namespace AdventOfCSharp.Benchmarking;

// Nullability is guaranteed given that the system operates as intended
#nullable disable

[MemoryDiagnoser]
public class ProblemBenchmark
{
    public int Year { get; set; }
    public int Day { get; set; }

    private ProblemRunner runner;
    private Action solverPart1, solverPart2;

    public ProblemBenchmark() { }

    [GlobalSetup]
    public void Setup()
    {
        var problemInfo = ProblemsIndex.Instance[Year, Day];
        runner = new ProblemRunner(problemInfo.InitializeInstance());
        solverPart1 = runner.NoReturnSolverActionForPart(1);
        solverPart2 = runner.NoReturnSolverActionForPart(2);
        runner.Problem.EnsureLoadedState();
    }

    [Benchmark]
    public void Input()
    {
        runner.Problem.LoadCurrentState();
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
