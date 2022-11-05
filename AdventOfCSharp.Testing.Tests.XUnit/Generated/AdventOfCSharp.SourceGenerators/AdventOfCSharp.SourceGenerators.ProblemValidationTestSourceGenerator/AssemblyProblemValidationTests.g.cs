
using Xunit;
using AdventOfCSharp;
using AdventOfCSharp.Testing.XUnit;

namespace AdventOfCSharp.Testing.Tests.XUnit;

public abstract class AssemblyProblemValidationTests<TProblem> : XUnitProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    protected override string GetProblemFileBaseDirectory() => @"..\..\..\..\AdventOfCSharp.ProblemSolutionResources";
}
