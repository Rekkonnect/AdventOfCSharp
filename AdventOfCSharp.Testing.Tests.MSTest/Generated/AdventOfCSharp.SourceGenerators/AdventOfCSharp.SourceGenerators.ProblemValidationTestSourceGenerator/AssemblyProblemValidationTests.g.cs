
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AdventOfCSharp;
using AdventOfCSharp.Testing.MSTest;

namespace AdventOfCSharp.Testing.Tests.MSTest;

public abstract class AssemblyProblemValidationTests<TProblem> : MSTestProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    protected override string GetProblemFileBaseDirectory() => @"..\..\..\..\AdventOfCSharp.ProblemSolutionResources";
}
