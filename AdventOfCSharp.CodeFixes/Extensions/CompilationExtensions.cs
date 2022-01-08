using Microsoft.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace AdventOfCSharp.CodeFixes.Extensions;

#nullable enable

public static class CompilationExtensions
{
    public static ISymbol? GetMatchingSymbol(this Compilation compilation, ISymbol? match, CancellationToken cancellationToken = default)
    {
        if (match is null)
            return null;

        var symbols = compilation.GetSymbolsWithName(match.Name, match.GetRespectiveSymbolFilter(), cancellationToken);
        return symbols.FirstOrDefault(symbol => symbol.MatchesKindAndFullContainingNamespace(match));
    }
}
