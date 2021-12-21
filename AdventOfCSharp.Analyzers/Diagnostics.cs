using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCSharp.Analyzers;

using static AoCSDiagnosticDescriptorStorage;

internal static class Diagnostics
{
    public static Diagnostic CreateAoCS0001(AttributeSyntax node)
    {
        return Diagnostic.Create(Instance[0001], node?.GetLocation());
    }
    public static Diagnostic CreateAoCS0002(AttributeArgumentSyntax node)
    {
        return Diagnostic.Create(Instance[0002], node?.GetLocation());
    }
}
