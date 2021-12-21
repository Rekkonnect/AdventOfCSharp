namespace AdventOfCSharp;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class PartSolverAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class PartSolutionAttribute : Attribute
{
    public PartSolutionStatus Status { get; }

    public PartSolutionAttribute(PartSolutionStatus status)
    {
        Status = status;
    }
}
