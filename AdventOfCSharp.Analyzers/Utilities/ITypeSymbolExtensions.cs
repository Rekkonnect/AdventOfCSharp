using Microsoft.CodeAnalysis;
using System.Linq;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class ITypeSymbolExtensions
{
    public static string FullMetadataName(this ITypeSymbol typeSymbol)
    {
        return $"{typeSymbol.ContainingNamespace.Name}.{typeSymbol.MetadataName}";
    }

    public static bool HasPublicParameterlessInstanceConstructor(this INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.InstanceConstructors.Any(IMethodSymbolExtensions.IsPublicParameterlessMethod);
    }
}
