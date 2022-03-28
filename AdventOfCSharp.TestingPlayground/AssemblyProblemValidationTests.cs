using AdventOfCSharp.Testing.MSTest;
using AdventOfCSharp.Testing.NUnit;
using AdventOfCSharp.Testing.XUnit;

namespace AdventOfCSharp.TestingPlayground;

public abstract class NUnitAssemblyProblemValidationTests<TProblem> : NUnitProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    protected override string GetProblemFileBaseDirectory()
    {
        return AoCSTestAssemblyInfo.ProblemFileBaseDirectory;
    }
}
public abstract class XUnitAssemblyProblemValidationTests<TProblem> : XUnitProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    protected override string GetProblemFileBaseDirectory()
    {
        return AoCSTestAssemblyInfo.ProblemFileBaseDirectory;
    }
}
public abstract class MSTestAssemblyProblemValidationTests<TProblem> : MSTestProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    protected override string GetProblemFileBaseDirectory()
    {
        return AoCSTestAssemblyInfo.ProblemFileBaseDirectory;
    }
}