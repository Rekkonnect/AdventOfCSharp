using BenchmarkDotNet.Attributes;
using System.Collections.Immutable;

namespace AdventOfCSharp.Benchmarking;

public abstract class BenchmarkDescriber
{
    protected static void CreateAssignBenchmarkedActions(Problem instance, ref Action part1, ref Action part2, ref Action input)
    {
        part1 = ProblemSolverMethodProvider.CreateSolverDelegate(1, instance);
        part2 = ProblemSolverMethodProvider.CreateSolverDelegate(2, instance);
        input = ProblemSolverMethodProvider.CreateLoadStateDelegate(instance);
    }
}

#region Attributes PoC
[Years(2022)]
[Dates(2016, 2, 4, 7, 15, 21, 25)]
[Dates(2017, 1, 23)]
[Parts(BenchmarkingParts.Input | BenchmarkingParts.Part2)]
public sealed partial class AttributesConsumer : BenchmarkDescriber
{

}
#endregion

#region PoC
[Years(2016, 2021)]
public sealed partial class Consumer : BenchmarkDescriber
{
}
#if DEBUG && true
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
    [BenchmarkCategory("Year 2021 Day 01")]
    public void Year2021_Day01_Part2()
    {
        year2021day1part2();
    }
}
#endif
#endregion