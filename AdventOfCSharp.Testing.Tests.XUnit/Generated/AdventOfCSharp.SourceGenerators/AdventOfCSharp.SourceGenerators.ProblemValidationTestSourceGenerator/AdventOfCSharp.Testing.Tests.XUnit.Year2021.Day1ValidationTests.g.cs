using Xunit;

namespace AdventOfCSharp.Testing.Tests.XUnit.Year2021;

public sealed partial class Day1ValidationTests : AssemblyProblemValidationTests<AdventOfCSharp.ProblemSolutionResources.Problems.Year2021.Day1>
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void PartTest(int part)
    {
        PartTestImpl(part);
    }
}
