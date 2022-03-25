using NUnit.Framework;

namespace AdventOfCSharp.AdditionalProblemSolutionTests;

public class DependingRunnerTests
{
    [Test]
    public void ValidateProblem()
    {
        // Transitively inherited problem type from dependency
        AssertExistence(2021, 1);
        // Implemented in this project
        AssertExistence(2021, 2);
    }
    private static void AssertExistence(int year, int day)
    {
        Assert.NotNull(ProblemsIndex.Instance[year, day].ProblemType.ProblemClass);
    }
}
