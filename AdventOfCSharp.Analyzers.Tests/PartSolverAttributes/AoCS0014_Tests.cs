using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.PartSolverAttributes;

[TestClass]
public sealed class AoCS0014_Tests : PartSolverAttributeAnalyzerTests
{
    [TestMethod]
    public void DuplicateCustomPartName()
    {
        var testCode =
@"
namespace AoC.Year2022;

public abstract class SomeCustomProblem : Problem<int>
{
    [PartSolver(""Part 3"", SolverKind = PartSolverKind.Custom)]
    public int SolvePart3() => -1;

    [PartSolver(↓""Part 4"", SolverKind = PartSolverKind.Custom)]
    public int SolvePart4A() => -1;
    [PartSolver(partName: ↓""Part 4"", SolverKind = PartSolverKind.Custom)]
    public int SolvePart4B() => -1;
}
";

        AssertDiagnosticsWithUsings(testCode);
    }

    [TestMethod]
    public void DuplicateInheritedPartName()
    {
        var testCode =
@"
namespace AoC.Year2022;

public abstract class SomeCustomProblem : Problem<int>
{
    [PartSolver(↓""Part 2"", SolverKind = PartSolverKind.Custom)]
    public int SolvePart2B() => -1;
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
