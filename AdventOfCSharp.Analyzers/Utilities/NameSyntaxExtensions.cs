using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography.X509Certificates;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class NameSyntaxExtensions
{
    public static SyntaxToken GetRightmostIdentifier(this NameSyntax nameSyntax)
    {
        return nameSyntax.GetRightmostNameSyntax().Identifier;
    }

    public static SimpleNameSyntax GetRightmostNameSyntax(this NameSyntax nameSyntax)
    {
        return nameSyntax switch
        {
            QualifiedNameSyntax qualifiedNameSyntax => qualifiedNameSyntax.Right,

            _ => GetSelfAsParentNameSyntaxOrThrow(nameSyntax),
        };
    }
    public static SimpleNameSyntax GetLeftmostNameSyntax(this NameSyntax nameSyntax)
    {
        return nameSyntax switch
        {
            QualifiedNameSyntax qualifiedNameSyntax => qualifiedNameSyntax.Left.GetLeftmostNameSyntax(),

            _ => GetSelfAsParentNameSyntaxOrThrow(nameSyntax),
        };
    }

    private static SimpleNameSyntax GetSelfAsParentNameSyntaxOrThrow(NameSyntax nameSyntax)
    {
        return GetSelfAsParentNameSyntax(nameSyntax) ?? throw null!;
    }
    private static SimpleNameSyntax? GetSelfAsParentNameSyntax(NameSyntax nameSyntax)
    {
        return nameSyntax switch
        {
            GenericNameSyntax genericNameSyntax => genericNameSyntax,
            SimpleNameSyntax simpleNameSyntax => simpleNameSyntax,
            AliasQualifiedNameSyntax aliasQualifiedNameSyntax => aliasQualifiedNameSyntax.Alias,

            _ => null,
        };
    }
}
