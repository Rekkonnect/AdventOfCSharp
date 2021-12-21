﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.PartSolutions;

[TestClass]
public sealed class AoCS0002_Tests : PartSolutionAnalyzerTests
{
    [TestMethod]
    public void InvalidPartSolutionStatusValues()
    {
        InvalidPartSolutionStatusValues(EnumLiteralCodeString(nameof(PartSolutionStatus.WIP)), false);
        InvalidPartSolutionStatusValues(EnumLiteralCodeString(nameof(PartSolutionStatus.Valid)), false);
        InvalidPartSolutionStatusValues(EnumLiteralCodeString(nameof(PartSolutionStatus.UnavailableFreeStar)), false);

        InvalidPartSolutionStatusValues(IntegerAsEnumLiteralCodeString(-1), true);
        InvalidPartSolutionStatusValues(IntegerAsEnumLiteralCodeString(1000), true);
        InvalidPartSolutionStatusValues(IntegerAsEnumLiteralCodeString(534789), true);
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