namespace AdventOfCSharp.Benchmarking;

public abstract class BenchmarkDescriberHelpers
{
    public static void CreateAssignBenchmarkedActions(Problem instance, ref Action part1, ref Action part2, ref Action input)
    {
        part1 = ProblemSolverMethodProvider.CreateSolverDelegate(1, instance);
        part2 = ProblemSolverMethodProvider.CreateSolverDelegate(2, instance);
        input = ProblemSolverMethodProvider.CreateLoadStateDelegate(instance);
    }
}