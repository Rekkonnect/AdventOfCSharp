using BenchmarkDotNet.Attributes;

namespace AdventOfCSharp.Benchmarking;

public abstract class BenchmarkDescriber
{
    protected virtual int[] Years => Array.Empty<int>();
    protected virtual int[] Days => Array.Empty<int>();
    protected virtual ProblemDate[] Dates => Array.Empty<ProblemDate>();

    public virtual BenchmarkingParts Parts => BenchmarkingParts.OnlyParts;

    internal static void CreateAssignBenchmarkedActions(Problem instance, ref Action part1, ref Action part2, ref Action input)
    {
        part1 = ProblemSolverMethodProvider.CreateSolverDelegate(1, instance);
        part2 = ProblemSolverMethodProvider.CreateSolverDelegate(2, instance);
        input = ProblemSolverMethodProvider.CreateLoadStateDelegate(instance);
    }
}

public enum BenchmarkingParts
{
    None = 0,

    Input = 1 << 0,
    Part1 = 1 << 1,
    Part2 = 1 << 2,

    OnlyParts = Part1 | Part2,

    All = Input | Part1 | Part2,
}

#region PoC
public sealed partial class Consumer : BenchmarkDescriber
{
    protected override int[] Years => new[] { 2016, 2021 };
}
#if DEBUG && false
#nullable disable
// Consumer.g.cs
partial class Consumer
{
    private readonly Problem year2021day1 = new AoC.Examples.Year2021.Day1();
    private Action year2021day1part1, year2021day1part2, year2021day1input;

    [GlobalSetup]
    public void Setup()
    {
        SetupActions();
    }

    private void SetupActions()
    {
        CreateAssignBenchmarkedActions(year2021day1, ref year2021day1part1, ref year2021day1part2, ref year2021day1input);
    }

    [Benchmark]
    [BenchmarkCategory("Year 2021 Day 01")]
    public void Year2021_Day01_Input()
    {
        year2021day1input();
    }
    [Benchmark]
    [BenchmarkCategory("Year 2021 Day 01")]
    public void Year2021_Day01_Part1()
    {
        year2021day1part1();
    }
    [Benchmark]
    [BenchmarkCategory("Year 2021 Day 02")]
    public void Year2021_Day01_Part2()
    {
        year2021day1part2();
    }
}
#endif
#endregion