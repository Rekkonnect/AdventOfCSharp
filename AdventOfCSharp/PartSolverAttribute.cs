namespace AdventOfCSharp;

/// <summary>Denotes that a method solves a problem's part. It applies to standard part 1 and 2 solvers, as well as custom part or easter egg solvers.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class PartSolverAttribute : Attribute { }
