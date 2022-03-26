namespace AdventOfCSharp.Benchmarking;

// Until BDN fixes reference issues, keep this here
internal abstract class BenchmarkDescriber
{
    protected static void CreateAssignBenchmarkedActions(Problem instance, ref Action part1, ref Action part2, ref Action input)
    {
        BenchmarkDescriberHelpers.CreateAssignBenchmarkedActions(instance, ref part1, ref part2, ref input);
    }
}
