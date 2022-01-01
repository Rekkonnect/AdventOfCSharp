using AdventOfCSharp.Analyzers.Utilities;
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

    public static Diagnostic CreateAoCS0004(ClassDeclarationSyntax declarationNode)
    {
        return CreateNamingConventionDiagnostic(declarationNode, 0004);
    }
    public static Diagnostic CreateAoCS0005(BaseNamespaceDeclarationSyntax declarationNode)
    {
        return CreateNamingConventionDiagnostic(declarationNode, 0005);
    }
    public static Diagnostic CreateAoCS0006(ClassDeclarationSyntax declarationNode)
    {
        return CreateNamingConventionDiagnostic(declarationNode, 0006);
    }
    public static Diagnostic CreateAoCS0007(ClassDeclarationSyntax declarationNode)
    {
        return CreateNamingConventionDiagnostic(declarationNode, 0007);
    }

    private static Diagnostic CreateNamingConventionDiagnostic(MemberDeclarationSyntax declarationNode, int code)
    {
        return Diagnostic.Create(Instance[code], declarationNode.GetIdentifierTokenOrNameSyntax().GetLocation());
    }

    public delegate Diagnostic UnmatchedNamingConventionDiagnosticCreator(ClassDeclarationSyntax declarationNode);
    public delegate Diagnostic InvalidDenotedDateDiagnosticCreator<in T>(T declarationNode)
        where T : MemberDeclarationSyntax;
}
