namespace AdventOfCSharp.Benchmarking;

public enum BenchmarkingParts
{
    None = 0,

    Input = 1 << 0,
    Part1 = 1 << 1,
    Part2 = 1 << 2,

    OnlyParts = Part1 | Part2,

    All = Input | OnlyParts,
}
