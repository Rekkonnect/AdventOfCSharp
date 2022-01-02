using Microsoft.CodeAnalysis;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class IMethodSymbolExtensions
{
    public static bool IsParameterlessNonGenericMethod(this IMethodSymbol symbol)
    {
        return symbol is
        {
            IsGenericMethod: false,
            Parameters.Length: 0,
        };
    }
}
