namespace AdventOfCSharp;

/// <summary>Denotes a part solution's status.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class PartSolutionAttribute : Attribute
{
    public PartSolutionStatus Status { get; }

    /// <summary>Initializes a new instance of the <seealso cref="PartSolutionAttribute"/>.</summary>
    /// <param name="status">The status of the part solution.</param>
    public PartSolutionAttribute(PartSolutionStatus status)
    {
        Status = status;
    }
}
