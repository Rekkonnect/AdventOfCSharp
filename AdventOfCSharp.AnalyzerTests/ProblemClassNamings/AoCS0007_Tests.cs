using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.ProblemClassNamings;

[TestClass]
public sealed class AoCS0007_Tests : ProblemClassNamingAnalyzerTests
{
    [TestMethod]
    public void Day0()
    {
        AssertClass("Day0", true);
    }
    [TestMethod]
    public void AfterChristmas()
    {
        AssertClass("Day26", true);
    }
    [TestMethod]
    public void Day1()
    {
        AssertClass("Day1", false);
    }
    [TestMethod]
    public void Day25()
    {
        AssertClass("Day25", false);
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
