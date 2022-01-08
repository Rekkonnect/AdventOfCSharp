using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCSharp.CodeFixes.Extensions;

#nullable enable

public static class INamespaceSymbolExtensions
{
    public static string FullNamespace(this INamespaceSymbol namespaceSymbol)
    {
        var namespaces = new List<string> { namespaceSymbol.Name };

        var current = namespaceSymbol.ContainingNamespace;
        while (current is not null)
        {
            namespaces.Add(current.Name);
            current = current.ContainingNamespace;
        }

        // LINQ isn't so convenient here
        namespaces.Reverse();
        var builder = new StringBuilder();
        foreach (var n in namespaces)
            builder.Append(n).Append('.');
        return builder.Remove(builder.Length - 1, 1).ToString();
    }
}
