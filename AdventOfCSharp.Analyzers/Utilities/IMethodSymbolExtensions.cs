using Microsoft.CodeAnalysis;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class IMethodSymbolExtensions
{
    public static bool IsPublicParameterlessMethod(this IMethodSymbol symbol)
    {
        return symbol is
        {
            DeclaredAccessibility: Accessibility.Public,
            Parameters.IsEmpty: true,
        };
    }
    public static bool IsParameterlessNonGenericMethod(this IMethodSymbol symbol)
    {
        return symbol is
        {
            IsGenericMethod: false,
            Parameters.IsEmpty: true,
        };
    }
}
