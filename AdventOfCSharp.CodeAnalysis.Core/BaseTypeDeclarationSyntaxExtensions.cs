using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn;

namespace AdventOfCSharp.CodeAnalysis.Core;

public static class BaseTypeDeclarationSyntaxExtensions
{
    public static string FullDeclaredSymbolName(this BaseTypeDeclarationSyntax typeDeclarationSyntax)
    {
        // Hate this type of code
        SyntaxNode current = typeDeclarationSyntax;
        var fullName = typeDeclarationSyntax.Identifier.Text;
        while (true)
        {
            var namespaceSyntax = current.GetNearestParentOfType<BaseNamespaceDeclarationSyntax>();
            if (namespaceSyntax is null)
                break;
            
            fullName = $"{namespaceSyntax.Name}.{fullName}";
            current = namespaceSyntax;
        }

        return fullName;
    }
}
