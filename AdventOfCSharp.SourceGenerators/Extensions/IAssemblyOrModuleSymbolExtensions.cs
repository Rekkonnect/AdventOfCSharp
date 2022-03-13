using Microsoft.CodeAnalysis;

#nullable enable

namespace AdventOfCSharp.SourceGenerators.Extensions;

public static class IAssemblyOrModuleSymbolExtensions
{
    public static INamespaceSymbol? GetGlobalNamespace(this ISymbol? symbol)
    {
        return symbol switch
        {
            IAssemblySymbol assemblySymbol => assemblySymbol.GlobalNamespace,
            IModuleSymbol moduleSymbol => moduleSymbol.GlobalNamespace,

            _ => null,
        };
    }
}
