using NUnit.Framework;

namespace AdventOfCSharp.Testing.NUnit;

public abstract class NUnitProblemValidationTests<TProblem> : FrameworkUnboundProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    protected sealed override void AssertEquality<T>(T expected, T actual)
    {
        Assert.AreEqual(expected, actual);
    }
}
