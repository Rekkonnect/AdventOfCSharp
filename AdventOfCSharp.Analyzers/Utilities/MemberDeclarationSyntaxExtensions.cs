using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class MemberDeclarationSyntaxExtensions
{
    public static SyntaxNodeOrToken GetIdentifierTokenOrNameSyntax(this MemberDeclarationSyntax declarationSyntax)
    {
        return declarationSyntax switch
        {
            BaseNamespaceDeclarationSyntax namespaceDeclaration => namespaceDeclaration.Name,
            BaseTypeDeclarationSyntax typeDeclarationSyntax => typeDeclarationSyntax.Identifier,

            PropertyDeclarationSyntax propertyDeclarationSyntax => propertyDeclarationSyntax.Identifier,
            EventDeclarationSyntax eventDeclarationSyntax => eventDeclarationSyntax.Identifier,
            // Fields use VariableDeclarationSyntax, and it doesn't reflect a single name

            MethodDeclarationSyntax methodDeclarationSyntax => methodDeclarationSyntax.Identifier,
            ConstructorDeclarationSyntax constructorDeclarationSyntax => constructorDeclarationSyntax.Identifier,

            // TODO: Ensure no forgotten types

            _ => default,
        };
    }
}
