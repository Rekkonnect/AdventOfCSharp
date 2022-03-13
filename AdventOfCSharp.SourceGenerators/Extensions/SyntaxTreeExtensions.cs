﻿using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCSharp.SourceGenerators.Extensions;

public static class SyntaxTreeExtensions
{
    public static IEnumerable<T> NodesOfType<T>(this SyntaxTree syntaxTree)
        where T : SyntaxNode
    {
        return syntaxTree.GetRoot().DescendantNodesAndSelf().OfType<T>();
    }
}
