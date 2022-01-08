using Microsoft.CodeAnalysis;

namespace AdventOfCSharp.CodeFixes.Extensions;

#nullable enable

public static class ISymbolExtensions
{
    public static string FullContainingNamespace(this ISymbol? symbol)
    {
        if (symbol is null)
            return "";

        return symbol.ContainingNamespace.FullNamespace();
    }

    public static bool MatchesKindAndFullContainingNamespace(this ISymbol symbol, ISymbol match)
    {
        return symbol.Kind == match.Kind
            && symbol.FullContainingNamespace() == match.FullContainingNamespace();
    }

    public static SymbolFilter GetRespectiveSymbolFilter(this ISymbol? symbol)
    {
        return symbol?.Kind switch
        {
            SymbolKind.Namespace => SymbolFilter.Namespace,

            SymbolKind.ArrayType or
            SymbolKind.DynamicType or
            SymbolKind.FunctionPointerType or
            SymbolKind.ErrorType or
            SymbolKind.NamedType or
            SymbolKind.PointerType or
            SymbolKind.TypeParameter => SymbolFilter.Type,

            SymbolKind.Method or
            SymbolKind.Property or
            SymbolKind.Field or
            SymbolKind.Event => SymbolFilter.Member,

            _ => SymbolFilter.None,
        };
    }
}
