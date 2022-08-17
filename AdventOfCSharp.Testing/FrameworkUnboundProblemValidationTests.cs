namespace AdventOfCSharp.Testing;

public abstract class FrameworkUnboundProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{
    private readonly ProblemRunner runner = new(new TProblem());

    protected FrameworkUnboundProblemValidationTests()
    {
        ExecutionTimePrinting.EnableLivePrinting = false;

        SetupProblemFileBaseDirectory();
    }

    protected void PartTestImpl(int part)
    {
        runner.Problem.ResetLoadedState();
        runner.Problem.EnsureLoadedState();

        var validation = runner.ValidatePart(part);
        AssertEquality(ValidationResult.Valid, validation?.Result);
    }

    protected abstract void AssertEquality<T>(T expected, T actual);

    protected void SetupProblemFileBaseDirectory()
    {
        ProblemFiles.CustomBaseDirectory = GetProblemFileBaseDirectory();
    }
    protected virtual string? GetProblemFileBaseDirectory() => null;
}
