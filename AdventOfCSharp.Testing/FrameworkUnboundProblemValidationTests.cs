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
        return;

        // In the case environment variables are needed
        var environmentBaseDirectory = ProblemFiles.ReadBaseDirectoryFromEnvironmentVariable();
        if (environmentBaseDirectory is not null)
            return;

        var directory = GetProblemFileBaseDirectory();
        ProblemFiles.SetCustomBaseDirectorySyncEnvironmentVariable(directory);
    }
    protected virtual string? GetProblemFileBaseDirectory() => null;
}
