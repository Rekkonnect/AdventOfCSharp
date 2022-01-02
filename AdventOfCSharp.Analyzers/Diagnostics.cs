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
        return CreateInvalidNumberDiagnostic(declarationNode, 0005, "Year");
    }
    public static Diagnostic CreateAoCS0006(ClassDeclarationSyntax declarationNode)
    {
        return CreateNamingConventionDiagnostic(declarationNode, 0006);
    }
    public static Diagnostic CreateAoCS0007(ClassDeclarationSyntax declarationNode)
    {
        return CreateInvalidNumberDiagnostic(declarationNode, 0007, "Day");
    }

    public static Diagnostic CreateAoCS0011(ClassDeclarationSyntax declarationNode)
    {
        return CreateBaseListFirstTypeDiagnostic(declarationNode, 0011);
    }
    public static Diagnostic CreateAoCS0012(ClassDeclarationSyntax declarationNode)
    {
        return CreateBaseListFirstTypeDiagnostic(declarationNode, 0012);
    }

    public static Diagnostic CreateAoCS0013(AttributeSyntax node)
    {
        return Diagnostic.Create(Instance[0013], node?.GetLocation());
    }

    private static Diagnostic CreateBaseListFirstTypeDiagnostic(BaseTypeDeclarationSyntax declarationNode, int code)
    {
        return Diagnostic.Create(Instance[code], declarationNode.BaseList.Types.First().GetLocation());
    }

    private static Diagnostic CreateNamingConventionDiagnostic(MemberDeclarationSyntax declarationNode, int code)
    {
        return Diagnostic.Create(Instance[code], declarationNode.GetIdentifierTokenOrNameSyntax().GetLocation());
    }
    private static Diagnostic CreateInvalidNumberDiagnostic(MemberDeclarationSyntax declarationNode, int code, string prefix)
    {
        var rightmost = declarationNode.GetIdentifierTokenOrNameSyntax();
        if (rightmost.AsNode() is NameSyntax name)
            rightmost = name.GetRightmostIdentifier();

        var wholeLocation = rightmost.GetLocation();
        var wholeSpan = wholeLocation.SourceSpan;
        var numberSpan = TextSpan.FromBounds(wholeSpan.Start + prefix.Length, wholeSpan.End);
        var numberLocation = Location.Create(declarationNode.SyntaxTree, numberSpan);
        return Diagnostic.Create(Instance[code], numberLocation);
    }

    public delegate Diagnostic FinalDayInheritanceDiagnosticCreator(ClassDeclarationSyntax declarationNode);

    public delegate Diagnostic UnmatchedNamingConventionDiagnosticCreator(ClassDeclarationSyntax declarationNode);
    public delegate Diagnostic InvalidDenotedDateDiagnosticCreator<in T>(T declarationNode)
        where T : MemberDeclarationSyntax;
}
