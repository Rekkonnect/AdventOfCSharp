using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.CodeFixes.Tests.FinalDayUsage;

[TestClass]
public class AoCS0011_CodeFixTests : FinalDayUserCodeFixTests
{
    [TestMethod]
    [DataTestMethod]
    [DataRow("int")]
    [DataRow("string")]
    [DataRow("ulong")]
    public void ConflictingConstraintsCodeFix(string type)
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day25 : {{|*:Problem<{type}, string>|}}
{{
    public override {type} SolvePart1() => default;
    public override string SolvePart2() => default;
}}
";

        var fixedCode =
$@"
namespace AoC.Year2021;

public class Day25 : FinalDay<{type}>
{{
    public override {type} SolvePart1() => default;
}}
";
        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
