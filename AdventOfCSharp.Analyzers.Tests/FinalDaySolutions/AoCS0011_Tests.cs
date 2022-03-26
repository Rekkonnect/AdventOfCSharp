using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.FinalDaySolutions;

[TestClass]
public sealed class AoCS0011_Tests : FinalDayAnalyzerTests
{
    [TestMethod]
    public void Problem1()
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day25 : ↓Problem<int>
{{
    public override int SolvePart1() => -1;
    public override int SolvePart2() => -2;
}}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [TestMethod]
    public void Problem2()
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day25 : ↓Problem<int, string>
{{
    public override int SolvePart1() => -1;
    public override string SolvePart2() => default;
}}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [TestMethod]
    public void FinalDay()
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
    public void CustomFinalDay()
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day25 : CustomFinalDay<int>
{{
    public override int SolvePart1() => -1;
}}

public abstract class CustomFinalDay<T> : FinalDay<T> {{ }}
";

        ValidateCodeWithUsings(testCode);
    }

    [TestMethod]
    public void NonProblemSolutionClass()
    {
        const string testCode =
@"
namespace AoC.Year2021

public class Day25 : NotProblem<int, string>
{
    public override int SolvePart1() => default;
    public override string SolvePart2() => default;
}
";

        ValidateCodeWithUsings(testCode);
    }
}
