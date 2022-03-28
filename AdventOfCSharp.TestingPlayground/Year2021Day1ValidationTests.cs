using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Xunit;

namespace AdventOfCSharp.TestingPlayground;

public class NUnitYear2021Day1ValidationTests : NUnitAssemblyProblemValidationTests<AdventOfCSharp.ProblemSolutionResources.Problems.Year2021.Day1>
{
    [Test]
    [TestCase(1)]
    [TestCase(2)]
    public void PartTest(int part)
    {
        PartTestImpl(part);
    }
}
public class XUnitYear2021Day1ValidationTests : XUnitAssemblyProblemValidationTests<AdventOfCSharp.ProblemSolutionResources.Problems.Year2021.Day1>
{
    [Xunit.Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void PartTest(int part)
    {
        PartTestImpl(part);
    }
}
[TestClass]
public class MSTestYear2021Day1ValidationTests : MSTestAssemblyProblemValidationTests<AdventOfCSharp.ProblemSolutionResources.Problems.Year2021.Day1>
{
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void PartTest(int part)
    {
        PartTestImpl(part);
    }
}