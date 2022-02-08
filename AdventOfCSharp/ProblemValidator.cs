namespace AdventOfCSharp;

public sealed class ProblemValidator
{
    public ValidationReport Report { get; } = new();

    public ValidationProblemResult Validate(Problem instance)
    {
        var runner = new ProblemRunner(instance);
        var result = runner.ValidateAllParts();
        Report.Add(instance, result);
        return result;
    }
    public ValidationProblemResult? Validate(ProblemType type)
    {
        var instance = type.InitializeInstance();
        if (instance is null)
            return null;

        return Validate(instance);
    }
    public ValidationProblemResult? Validate(ProblemInfo info)
    {
        return Validate(info.ProblemType);
    }
    public ValidationProblemResult? Validate(int year, int day)
    {
        return Validate(ProblemsIndex.Instance[year, day]);
    }
}
