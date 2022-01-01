using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.ProblemClassNamings;

[TestClass]
public sealed class AoCS0006_Tests : ProblemClassNamingAnalyzerTests
{
    [TestMethod]
    public void StartsButNotEnds()
    {
        AssertClass("Day1F", true);
    }
    [TestMethod]
    public void NotStartsButEnds()
    {
        AssertClass("NotDay1", true);
    }
    [TestMethod]
    public void OnlyContains()
    {
        AssertClass("NotDay1F", true);
    }
    [TestMethod]
    public void IrrelevantName()
    {
        AssertClass("Irrelevant", true);
    }

    private void AssertClass(string className, bool assert)
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class ↓{className} : Problem<int>
{{
    public override int SolvePart1() => -1;
    public override int SolvePart2() => -2;
}}
";

        AssertOrValidateWithUsings(testCode, assert);
    }
}
