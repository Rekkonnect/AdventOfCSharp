using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.FinalDaySolutions;

[TestClass]
public sealed class AoCS0012_Tests : FinalDayAnalyzerTests
{
    [TestMethod]
    public void FinalDay()
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day2 : ↓FinalDay<int>
{{
    public override int SolvePart1() => -1;
}}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [TestMethod]
    public void InvalidCustomFinalDay()
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day2 : ↓CustomFinalDay<int>
{{
    public override int SolvePart1() => -1;
}}

public abstract class CustomFinalDay<T> : FinalDay<T> {{ }}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
