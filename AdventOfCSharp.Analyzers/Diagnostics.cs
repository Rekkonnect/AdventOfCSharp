using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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
    public static Diagnostic CreateAoCS0003(GenericNameSyntax baseType)
    {
        // Bonus points for location accuracy?
        // TODO: Abstract this away, maybe to RoseLynn
        var typeArgumentList = baseType.TypeArgumentList;
        var arguments = typeArgumentList.Arguments;
        var firstSeparator = arguments.GetSeparator(0);
        var start = firstSeparator.Span;
        var end = arguments.Last().Span;
        var fullSpan = TextSpan.FromBounds(start.Start, end.End);
        var location = Location.Create(typeArgumentList.SyntaxTree, fullSpan);

        return Diagnostic.Create(Instance[0003], location);
    }
}
