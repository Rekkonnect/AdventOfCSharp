using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AdventOfCSharp.Analyzers.Tests.ProblemClassNamings;

[TestClass]
public sealed class AoCS0005_Tests : ProblemClassNamingAnalyzerTests
{
    [TestMethod]
    public void TooEarly()
    {
        AssertNamespace(2014, true);
    }
    [TestMethod]
    public void Starting2015()
    {
        AssertNamespace(2015, false);
    }
    [TestMethod]
    public void TooShort()
    {
        AssertNamespace(20, true);
    }
    [TestMethod]
    public void TooLong()
    {
        AssertNamespace(204512, true);
    }
    [TestMethod]
    public void CurrentYear()
    {
        AssertNamespace(DateTime.Now.Year, false);
    }
    [TestMethod]
    public void NextYear()
    {
        AssertNamespace(DateTime.Now.Year + 1, true);
    }

    private void AssertNamespace(int year, bool assert)
    {
        var testCode =
$@"
namespace ↓AoC.Year{year};

public class Day1 : Problem<int>
{{
    public override int SolvePart1() => -1;
    public override int SolvePart2() => -2;
}}
";

        AssertOrValidateWithUsings(testCode, assert);
    }
}
