using System;

namespace AdventOfCSharp.Testing;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class AoCSTestAssemblyAttribute : Attribute
{
    public string ProblemFileBaseDirectory { get; }

    public AoCSTestAssemblyAttribute(string problemFileBaseDirectory)
    {
        ProblemFileBaseDirectory = problemFileBaseDirectory;
    }
}
