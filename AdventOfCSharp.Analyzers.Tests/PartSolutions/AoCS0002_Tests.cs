using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.PartSolutions;

[TestClass]
public sealed class AoCS0002_Tests : PartSolutionAnalyzerTests
{
    [TestMethod]
    public void InvalidPartSolutionStatusValues()
    {
        InvalidPartSolutionStatusValues(EnumLiteralCodeString(nameof(PartSolutionStatus.WIP)), false);
        InvalidPartSolutionStatusValues(EnumLiteralCodeString(nameof(PartSolutionStatus.Valid)), false);
        InvalidPartSolutionStatusValues(EnumLiteralCodeString(nameof(PartSolutionStatus.UnavailableLockedStar)), false);

        InvalidPartSolutionStatusValues(IntegerAsEnumLiteralCodeString(-1), true);
        InvalidPartSolutionStatusValues(IntegerAsEnumLiteralCodeString(1000), true);
        InvalidPartSolutionStatusValues(IntegerAsEnumLiteralCodeString(534789), true);
    }

    [TestMethod]
    public void IrrelevantAttribute()
    {
        string testCode =
@"
namespace AoC.Year2021;

public class Day1 : Problem<int>
{
    [Random((DayOfWeek)456)]
    public override int SolvePart1() => -1;
    [Random((DayOfWeek)789)]
    public override int SolvePart2() => -2;
}

public sealed class RandomAttribute : Attribute
{
    public RandomAttribute(DayOfWeek day) { }
}
";

        ValidateCodeWithUsings(testCode);

    }

    private void InvalidPartSolutionStatusValues(string enumValueCode, bool assert)
    {
        string testCode =
$@"
namespace AoC.Year2021;

public class Day1 : Problem<int>
{{
    [PartSolution(↓{enumValueCode})]
    public override int SolvePart1() => -1;
    [PartSolution(↓{enumValueCode})]
    public override int SolvePart2() => -2;
}}
";

        AssertOrValidateWithUsings(testCode, assert);
    }
    private static string EnumLiteralCodeString(string valueName)
    {
        return $"{nameof(PartSolutionStatus)}.{valueName}";
    }
    private static string IntegerAsEnumLiteralCodeString(int value)
    {
        return $"({nameof(PartSolutionStatus)})({value})";
    }
}
