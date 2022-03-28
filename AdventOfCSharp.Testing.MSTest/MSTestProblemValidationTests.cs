using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Testing.MSTest;

public abstract class MSTestProblemValidationTests<TProblem> : FrameworkUnboundProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    protected sealed override void AssertEquality<T>(T expected, T actual)
    {
        Assert.AreEqual(expected, actual);
    }
}