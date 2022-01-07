using Microsoft.CodeAnalysis;
using System.Linq;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class ISymbolExtensions
{
    public static AttributeData? FirstOrDefaultAttribute(this ISymbol symbol, string name)
    {
        return symbol.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass?.Name == name);
    }
}
