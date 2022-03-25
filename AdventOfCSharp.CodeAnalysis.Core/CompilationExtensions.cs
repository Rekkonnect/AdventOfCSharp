using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCSharp.CodeAnalysis.Core;

public static class CompilationExtensions
{
    public static IEnumerable<T> NodesOfType<T>(this Compilation compilation)
        where T : SyntaxNode
    {
        return compilation.SyntaxTrees.Select(tree => tree.NodesOfType<T>()).SelectMany(nodes => nodes);
    }
}
