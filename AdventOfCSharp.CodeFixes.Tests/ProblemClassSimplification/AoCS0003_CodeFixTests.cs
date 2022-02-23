using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.CodeFixes.Tests.ProblemClassSimplification;

[TestClass]
public class AoCS0003_CodeFixTests : ProblemClassSimplifierCodeFixTests
{
    [DataTestMethod]
    [DataRow("int")]
    [DataRow("string")]
    [DataRow("ulong")]
    public void ExpandedProblemCodeFix(string type)
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day1 : Problem<{type}{{|*:, {type}|}}>
{{
    public override {type} SolvePart1() => default;
    public override {type} SolvePart2() => default;
}}
";

        var fixedCode =
$@"
namespace AoC.Year2021;

public class Day1 : Problem<{type}>
{{
    public override {type} SolvePart1() => default;
    public override {type} SolvePart2() => default;
}}
";
        TestCodeFixWithUsings(testCode, fixedCode);
    }

    [DataTestMethod]
    [DataRow("int")]
    [DataRow("string")]
    [DataRow("ulong")]
    public void ExpandedProblemBatchCodeFix(string type)
    {
        var testCode =
$@"
namespace AoC.Year2021;

public class Day1 : Problem<{type}{{|*:, {type}|}}>
{{
    public override {type} SolvePart1() => default;
    public override {type} SolvePart2() => default;
}}
public class Day2 : Problem<{type}{{|*:, {type}|}}>
{{
    public override {type} SolvePart1() => default;
    public override {type} SolvePart2() => default;
}}
public class Day3 : Problem<{type}{{|*:, {type}|}}>
{{
    public override {type} SolvePart1() => default;
    public override {type} SolvePart2() => default;
}}
";

        var fixedCode =
$@"
namespace AoC.Year2021;

public class Day1 : Problem<{type}>
{{
    public override {type} SolvePart1() => default;
    public override {type} SolvePart2() => default;
}}
public class Day2 : Problem<{type}>
{{
    public override {type} SolvePart1() => default;
    public override {type} SolvePart2() => default;
}}
public class Day3 : Problem<{type}>
{{
    public override {type} SolvePart1() => default;
    public override {type} SolvePart2() => default;
}}
";
        TestCodeFixWithUsings(testCode, fixedCode);
    }
}
