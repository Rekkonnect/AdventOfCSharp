using System;
using System.ComponentModel;

namespace AdventOfCSharp;

/// <summary>Denotes that a method solves a problem's part. It applies to standard part 1 and 2 solvers, as well as custom part or easter egg solvers.</summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class PartSolverAttribute : Attribute
{
    /// <summary>The name of the part.</summary>
    public string PartName { get; }

    /// <summary>Gets the <seealso cref="PartSolverKind"/> for this part solver.</summary>
    public PartSolverKind SolverKind { get; set; } = PartSolverKind.Official;

    public PartSolverAttribute(string partName)
    {
        PartName = partName;
    }
}

/// <summary>Represents the part solver kind.</summary>
public enum PartSolverKind
{
    /// <summary>Represents a solver for an official part (part 1 or 2).</summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    Official,
    /// <summary>Represents a solver for a custom part (usually fanmade "part 3").</summary>
    Custom,
    /// <summary>Represents a solver for an easter egg.</summary>
    EasterEgg,
}
