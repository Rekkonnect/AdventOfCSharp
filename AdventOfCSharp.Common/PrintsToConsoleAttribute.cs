using System;

namespace AdventOfCSharp;

/// <summary>Denotes that a method prints to the console, used to avoid conflicts when multiple sources are printing.</summary>
/// <remarks>Using this attribuute hints <seealso cref="ProblemRunner"/> to temporarily disable live execution display.</remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class PrintsToConsoleAttribute : Attribute
{
}
