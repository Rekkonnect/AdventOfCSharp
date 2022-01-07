using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.ProblemClassNamings;

[TestClass]
public sealed class AoCS0004_Tests : ProblemClassNamingAnalyzerTests
{
    [TestMethod]
    public void ParentMatches()
    {
        AssertNamespace("AoC.Year2021.Specific", true);
    }
    [TestMethod]
    public void StartsButNotEnds()
    {
        AssertNamespace("AoC.Year2021F", true);
    }
    [TestMethod]
    public void NotStartsButEnds()
    {
        AssertNamespace("AoC.NotYear2021", true);
    }
    [TestMethod]
    public void OnlyContains()
    {
        AssertNamespace("AoC.NotYear2021F", true);
    }
    [TestMethod]
    public void IrrelevantName()
    {
        AssertNamespace("AoC.Blah", true);
    }

    [TestMethod]
    public void NonProblemSolutionClass()
    {
        const string testCode =
@"
namespace AoC.NotYear2021F.Something

public class Day1 : NotProblem<int>
{
    public override int SolvePart1() => default;
    public override int SolvePart2() => default;
}
";

        ValidateCodeWithUsings(testCode);
    }

    private void AssertNamespace(string namespaceString, bool assert)
    {
        var testCode =
$@"
namespace {namespaceString};

public class ↓Day1 : Problem<int>
{{
    public override int SolvePart1() => -1;
    public override int SolvePart2() => -2;
}}
";

        AssertOrValidateWithUsings(testCode, assert);
    }
}
