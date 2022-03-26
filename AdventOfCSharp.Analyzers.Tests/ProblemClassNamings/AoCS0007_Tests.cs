using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.ProblemClassNamings;

[TestClass]
public sealed class AoCS0007_Tests : ProblemClassNamingAnalyzerTests
{
    protected override void AssertDiagnostics(string testCode)
    {
        AssertDiagnosticsMicrosoftCodeAnalysis(testCode);
    }

    [TestMethod]
    public void Day0()
    {
        AssertClass(0, true);
    }
    [TestMethod]
    public void AfterChristmas()
    {
        AssertClass(26, true);
    }
    [TestMethod]
    public void Day1()
    {
        AssertClass(1, false);
    }
    [TestMethod]
    public void Day25()
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day25 : FinalDay<int>
{{
    public override int SolvePart1() => -1;
}}
";

        ValidateCodeWithUsings(testCode);
    }
    [TestMethod]
    public void TooLong()
    {
        AssertClass(4531, true);
    }

    [TestMethod]
    public void NonProblemSolutionClass()
    {
        const string testCode =
@"
namespace AoC.Year2021

public class Day124678 : NotProblem<int>
{
    public override int SolvePart1() => default;
    public override int SolvePart2() => default;
}
";

        ValidateCodeWithUsings(testCode);
    }

    private void AssertClass(int day, bool assert)
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day{{|*:{day}|}} : Problem<int>
{{
    public override int SolvePart1() => -1;
    public override int SolvePart2() => -2;
}}
";

        AssertOrValidateWithUsings(testCode, assert);
    }
}
