using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCSharp.SourceGenerators.Extensions;

public static class CompilationExtensions
{
    public static IEnumerable<T> NodesOfType<T>(this Compilation compilation)
        where T : SyntaxNode
    {
        return compilation.SyntaxTrees.Select(tree => tree.NodesOfType<T>()).SelectMany(nodes => nodes);
    }

    public static IEnumerable<INamedTypeSymbol> GetAllDefinedTypes(this Compilation compilation)
    {
        return compilation.Assembly.GlobalNamespace.GetAllContainedTypes();
    }

    public static IEnumerable<INamedTypeSymbol> GetAllContainedTypes(this INamespaceSymbol namespaceSymbol)
    {
        var types = namespaceSymbol.GetTypeMembers();
        var namespaces = namespaceSymbol.GetNamespaceMembers();
        return types.Concat(namespaces.SelectMany(GetAllContainedTypes));
    }

    public static IEnumerable<ISymbol> GetAllSymbols(this Compilation compilation)
    {
        return compilation.References.Select(compilation.GetAssemblyOrModuleSymbol).Where(s => s is not null) as IEnumerable<ISymbol>;
    }
}
