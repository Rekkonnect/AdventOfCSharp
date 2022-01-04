using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.ProblemSolutionClassInheritance;

[TestClass]
public sealed class AoCS0003_Tests : ProblemInheritanceAnalyzerTests
{
    // TODO: Assert diagnostic span using enclosing notation
    [TestMethod]
    public void RedundantReturnTypeRepetition()
    {
        const string testCode =
@"
namespace AoC.Year2021;

public class Day1 : Problem<int↓, int>
{
    public override int SolvePart1() => -1;
    public override int SolvePart2() => -2;
}
public abstract class ProblemIntString : Problem<int, string>
{
}
public abstract class ProblemStringString : Problem<string↓, string>
{
}
public abstract class ProblemGlyphGrid : Problem<IGlyphGrid↓, IGlyphGrid>
{
}
public abstract class ProblemString : Problem<string>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [TestMethod]
    public void RedundantReturnTypeRepetitionPartial()
    {
        const string testCode =
@"
namespace AoC.Year2021;

public partial class Day1 : Problem<int↓, int>
{
    public override int SolvePart1() => -1;
}
public partial class Day1 : Problem<int↓, int>
{
    public override int SolvePart2() => -2;
}
public partial class Day1 : Problem<int↓, int>
{
}
public partial class Day1
{
}

public class Day2 : Problem<int>
{
}
";

        AssertDiagnosticsWithUsings(testCode);
    }

    [TestMethod]
    public void IrrelevantTypeParameterRepetition()
    {
        const string testCode =
@"
namespace AoC.Year2021;

public class Day1 : Dictionary<int, int>
{
}
public class IntDictionary : Dictionary<int, int>
{
}
";

        ValidateCodeWithUsings(testCode);
    }
}
