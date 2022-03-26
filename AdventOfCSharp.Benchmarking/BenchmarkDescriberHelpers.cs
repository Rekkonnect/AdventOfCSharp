namespace AdventOfCSharp.Benchmarking;

// sealed to allow being used as a type argument; static in reality
public sealed class BenchmarkDescriberHelpers
{
    public static void CreateAssignBenchmarkedActions(Problem instance, ref Action part1, ref Action part2, ref Action input)
    {
        part1 = ProblemSolverMethodProvider.CreateNoReturnSolverDelegate(1, instance);
        part2 = ProblemSolverMethodProvider.CreateNoReturnSolverDelegate(2, instance);
        input = ProblemSolverMethodProvider.CreateLoadStateDelegate(instance);
    }
}