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
    [↓PartSolver(""Solver"", SolverKind = PartSolverKind.Custom)]
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

    [↓PartSolver(""Parameter"", SolverKind = PartSolverKind.Custom)]
    public int Parameter(int x) => x;
    [↓PartSolver(""Generic"", SolverKind = PartSolverKind.Custom)]
    public int Generic<T>() => -1;
    [↓PartSolver(""Void"", SolverKind = PartSolverKind.Custom)]
    public void Void() { }

    [↓PartSolver(""Static"", SolverKind = PartSolverKind.Custom)]
    public static int Static() => -1;

    [↓PartSolver(""ProtectedInternal"", SolverKind = PartSolverKind.Custom)]
    protected internal int ProtectedInternal() => -1;
    [↓PartSolver(""Internal"", SolverKind = PartSolverKind.Custom)]
    internal int Internal() => -1;
    [↓PartSolver(""Protected"", SolverKind = PartSolverKind.Custom)]
    protected int Protected() => -1;
    [↓PartSolver(""PrivateProtected"", SolverKind = PartSolverKind.Custom)]
    private protected int PrivateProtected() => -1;
    [↓PartSolver(""Private"", SolverKind = PartSolverKind.Custom)]
    private int Private() => -1;
}
";

        AssertDiagnosticsWithUsings(testCode);
    }

    [TestMethod]
    public void Problem2Declarer()
    {
        var testCode =
@"
public abstract class Problem<T1, T2> : Problem
{
    [PartSolver(""Part 1"", SolverKind = PartSolverKind.Custom)]
    public abstract T1 SolvePart1();
    [PartSolver(""Part 2"", SolverKind = PartSolverKind.Custom)]
    public abstract T2 SolvePart2();
}
";

        ValidateCodeWithUsings(testCode);
    }
}
