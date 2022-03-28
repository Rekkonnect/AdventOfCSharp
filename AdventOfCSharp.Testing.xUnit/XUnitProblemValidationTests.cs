using Xunit;

namespace AdventOfCSharp.Testing.XUnit;

public abstract class XUnitProblemValidationTests<TProblem> : FrameworkUnboundProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    protected sealed override void AssertEquality<T>(T expected, T actual)
    {
        Assert.Equal(expected, actual);
    }
}