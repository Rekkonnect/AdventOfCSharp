using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn.CSharp.Syntax;
using System.Linq;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class AttributeSyntaxExtensions
{
    public static AttributeData? GetAttributeData(this AttributeSyntax attributeSyntax, SemanticModel semanticModel)
    {
        return attributeSyntax.GetAttributeData(semanticModel, out _);
    }
    public static AttributeData? GetAttributeData(this AttributeSyntax attributeSyntax, SemanticModel semanticModel, out ISymbol? attributedSymbol)
    {
        var solverDeclarationNode = attributeSyntax.GetAttributeDeclarationParent()!;
        attributedSymbol = semanticModel.GetDeclaredSymbol(solverDeclarationNode);
        return attributedSymbol?.GetAttributes().FirstOrDefault(MatchesAttributeData);

        bool MatchesAttributeData(AttributeData attribute)
        {
            return attribute.ApplicationSyntaxReference!.GetSyntax() == attributeSyntax;
        }
    }

    public static SyntaxNode? GetAttributeDeclarationParent(this AttributeSyntax attributeSyntax)
    {
        return attributeSyntax.GetParentAttributeList().Parent;
    }
}
