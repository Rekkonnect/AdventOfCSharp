
using NUnit.Framework;
using AdventOfCSharp;
using AdventOfCSharp.Testing.NUnit;

namespace AdventOfCSharp.Testing.Tests.NUnit;

public abstract class AssemblyProblemValidationTests<TProblem> : NUnitProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    protected override string GetProblemFileBaseDirectory() => @"..\..\..\..\AdventOfCSharp.ProblemSolutionResources";
}
