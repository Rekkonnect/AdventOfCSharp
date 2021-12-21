using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.PartSolutions;

[TestClass]
public sealed class AoCS0001_Tests : PartSolutionAnalyzerTests
{
    [TestMethod]
    public void InvalidPartSolutionAttribute()
    {
        const string testCode =
@"
namespace AoC.Year2021;

public class Day1 : Problem<int>
{
    [PartSolution(PartSolutionStatus.WIP)]
    public override int SolvePart1() => -1;
    [PartSolution(PartSolutionStatus.WIP)]
    public override int SolvePart2() => -2;

    [↓PartSolution(PartSolutionStatus.WIP)]
    public static void RandomFunction() { }

    [PartSolution(PartSolutionStatus.WIP)]
    [PartSolver]
    public int RandomSolver() => 1;
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
