using NUnit.Framework;

namespace AdventOfCSharp.ProblemSolutionTests;

public class ProblemRunnerTests
{
    /*
     * So far, this test alone proves that the following work right:
     * - Input loading
     * - Output loading
     * - Problem running
     * - Test case loading
     * - Output validation
     */
    [Test]
    public void ValidateProblem()
    {
        var runner = ProblemRunner.ForProblem(2021, 1)!;
        runner.Options.DisplayExecutionTimes = false;
        Assert.False(runner.FullyValidateAllTestCases().HasInvalidResults);
    }
}
