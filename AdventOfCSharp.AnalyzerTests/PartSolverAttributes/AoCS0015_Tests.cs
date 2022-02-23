using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.PartSolverAttributes;

[TestClass]
public sealed class AoCS0015_Tests : PartSolverAttributeAnalyzerTests
{
    [TestMethod]
    public void OverflowingCharacters()
    {
        var testCode =
@"
namespace AoC.Year2022;

public abstract class SomeCustomProblem : Problem<int>
{
    [PartSolver(""01234567890123456789"", SolverKind = PartSolverKind.EasterEgg)]
    public int Exactly20() => -1;
    [PartSolver(↓""01234567890123456789abc"", SolverKind = PartSolverKind.EasterEgg)]
    public int Over20() => -1;
    [PartSolver(partName: ↓""01234567890123456789abc"", SolverKind = PartSolverKind.EasterEgg)]
    public int NamedOver20() => -1;
    [PartSolver(""0123456789\x69123456789"", SolverKind = PartSolverKind.EasterEgg)]
    public int EscapedCharactersExactly20() => -1;
    [PartSolver(""\x69\x69\x69\x69\x69\x69\x69\x69\x69\x69"", SolverKind = PartSolverKind.EasterEgg)]
    public int EscapedCharactersExactly10() => -1;
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
