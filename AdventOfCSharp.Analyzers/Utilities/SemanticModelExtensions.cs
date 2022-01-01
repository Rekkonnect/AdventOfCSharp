﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class SemanticModelExtensions
{
    public static TSymbol? GetSymbol<TSymbol>(this SemanticModel semanticModel, TypeSyntax type)
        where TSymbol : class, ISymbol
    {
        return semanticModel.GetSymbolInfo(type).Symbol as TSymbol;
    }
}