using RoseLynn.CodeFixes;
using System.Resources;

namespace AdventOfCSharp.CodeFixes;

public abstract class AoCSCodeFixProvider : MultipleDiagnosticCodeFixProvider
{
    protected sealed override ResourceManager ResourceManager => CodeFixStringResources.ResourceManager;
}
