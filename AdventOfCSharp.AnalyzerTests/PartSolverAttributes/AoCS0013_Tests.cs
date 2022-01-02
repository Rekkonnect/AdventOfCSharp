using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.PartSolverAttributes;

[TestClass]
public sealed class AoCS0013_Tests : PartSolverAttributeAnalyzerTests
{
    [TestMethod]
    public void NotProblemClass()
    {
        var testCode =
@"
namespace AoC.Random;

public class Nothing : NotProblem<int>
{
    [↓PartSolver]
    public int Solver() => -1;
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [TestMethod]
    public void InvalidSignaturePartSolvers()
    {
        var testCode =
@"
namespace AoC.Year2021;

public class Day1 : Problem<int>
{
    public override int SolvePart1() => -1;
    public override int SolvePart2() => -2;

    [↓PartSolver]
    public int Parameter(int x) => x;
    [↓PartSolver]
    public int Generic<T>() => -1;
    [↓PartSolver]
    public void Void() { }

    [↓PartSolver]
    public static int Static() => -1;

    [↓PartSolver]
    protected internal int ProtectedInternal() => -1;
    [↓PartSolver]
    internal int Internal() => -1;
    [↓PartSolver]
    protected int Protected() => -1;
    [↓PartSolver]
    private protected int PrivateProtected() => -1;
    [↓PartSolver]
    private int Private() => -1;
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
