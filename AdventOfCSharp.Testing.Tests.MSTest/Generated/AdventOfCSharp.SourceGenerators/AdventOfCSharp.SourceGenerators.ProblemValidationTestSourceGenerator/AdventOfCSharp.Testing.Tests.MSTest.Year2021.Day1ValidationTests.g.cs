using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Testing.Tests.MSTest.Year2021;

[TestClass]
public sealed partial class Day1ValidationTests : AssemblyProblemValidationTests<AdventOfCSharp.ProblemSolutionResources.Problems.Year2021.Day1>
{
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void PartTest(int part)
    {
        PartTestImpl(part);
    }
}
