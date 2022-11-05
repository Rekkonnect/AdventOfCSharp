using NUnit.Framework;

namespace AdventOfCSharp.Testing.Tests.NUnit.Year2021;

public sealed partial class Day1ValidationTests : AssemblyProblemValidationTests<AdventOfCSharp.ProblemSolutionResources.Problems.Year2021.Day1>
{
    [Test]
    [TestCase(1)]
    [TestCase(2)]
    public void PartTest(int part)
    {
        PartTestImpl(part);
    }
}
