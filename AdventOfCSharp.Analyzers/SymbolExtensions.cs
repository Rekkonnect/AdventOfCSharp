using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

public static class SymbolExtensions
{
    public static bool HasAttributeMatchingType<T>(this ISymbol symbol)
        where T : Attribute
    {
        return HasAttributeNamed(symbol, typeof(T).Name);
    }
    public static bool HasAttributeNamed(this ISymbol symbol, string attributeClassName)
    {
        return symbol.GetAttributes().Any(attribute => attribute.AttributeClass.Name == attributeClassName);
    }

    public static bool HasInheritedAttributeMatchingType<T>(this IMethodSymbol symbol)
        where T : Attribute
    {
        return HasInheritedAttributeNamed(symbol, typeof(T).Name);
    }
    public static bool HasInheritedAttributeNamed(this IMethodSymbol symbol, string attributeClassName)
    {
        var current = symbol;
        while (current is not null)
        {
            if (HasAttributeNamed(current, attributeClassName))
                return true;

            current = current.OverriddenMethod;
        }
        return false;
    }
}
