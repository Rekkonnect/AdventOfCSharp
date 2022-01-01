using Microsoft.CodeAnalysis;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class ITypeSymbolExtensions
{
    public static string FullMetadataName(this ITypeSymbol typeSymbol)
    {
        return $"{typeSymbol.ContainingNamespace.Name}.{typeSymbol.MetadataName}";
    }
}
