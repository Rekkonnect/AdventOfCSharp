using AdventOfCSharp.ProblemSolutionResources;
using NUnit.Framework;

namespace AdventOfCSharp.Tests;

public class ProblemRunnerTests
{
    [SetUp]
    public void Setup()
    {
        ResourceFileManagement.SetResourceProjectAsBaseProblemFileDirectory();
    }

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
        var runner = ProblemRunner.ForProblem(2021, 1);
        Assert.True(runner.FullyValidateAllTestCases());
    }
}
