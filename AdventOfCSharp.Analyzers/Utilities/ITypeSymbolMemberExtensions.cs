using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCSharp.Analyzers.Utilities;

public static class ITypeSymbolMemberExtensions
{
    public static IEnumerable<ISymbol> GetAllInheritedMembers(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.GetAllBaseTypesAndInterfaces().SelectMany(type => type.GetMembers());
    }
    public static IEnumerable<ISymbol> GetAllMembersIncludingInherited(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().Concat(typeSymbol.GetAllInheritedMembers());
    }
    public static IEnumerable<ISymbol> GetAllInheritedInterfaceMembers(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.AllInterfaces.SelectMany(type => type.GetMembers());
    }
}
