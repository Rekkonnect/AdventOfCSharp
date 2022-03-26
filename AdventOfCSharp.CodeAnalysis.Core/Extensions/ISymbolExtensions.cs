using AdventOfCSharp.CodeAnalysis.Core.Extensions;
using Microsoft.CodeAnalysis;
using RoseLynn;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCSharp.CodeAnalysis.Core.Extensions;

#nullable enable

public static class ISymbolExtensions
{
    public static bool HasAttributeNamed<T>(this ISymbol symbol)
        where T : Attribute
    {
        return symbol.FirstOrDefaultAttributeNamed<T>() is not null;
    }
    public static AttributeData? FirstOrDefaultAttributeNamed<T>(this ISymbol symbol)
        where T : Attribute
    {
        return symbol.FirstOrDefaultAttributeNamed(typeof(T).Name);
    }
    public static IEnumerable<AttributeData> GetAttributesNamed<T>(this ISymbol symbol)
        where T : Attribute
    {
        return symbol.GetAttributes().Where(attribute => attribute.AttributeClass?.Name == typeof(T).Name);
    }

    // TODO: Implement for completeness in RoseLynn
    private static AttributeData? HasAttributeNamedFully<T>(this ISymbol symbol, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace)
        where T : Attribute
    {
        return null;
    }
    private static AttributeData? FirstOrDefaultAttributeNamedFully<T>(this ISymbol symbol, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace)
        where T : Attribute
    {
        return null;
    }
    private static IEnumerable<AttributeData> GetAttributesNamedFully<T>(this ISymbol symbol, SymbolNameMatchingLevel matchingLevel = SymbolNameMatchingLevel.Namespace)
        where T : Attribute
    {
        return null!;
    }
}
