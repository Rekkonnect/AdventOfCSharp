using Microsoft.CodeAnalysis;

#nullable enable

namespace AdventOfCSharp.CodeAnalysis.Core.Extensions;

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
