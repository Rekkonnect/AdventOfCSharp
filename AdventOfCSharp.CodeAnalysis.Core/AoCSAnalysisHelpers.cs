using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Linq;

namespace AdventOfCSharp.CodeAnalysis.Core;

public static class AoCSAnalysisHelpers
{
    public static bool IsImportantAoCSClass(INamedTypeSymbol classSymbol, string name)
    {
        var parsed = IdentifierWithArity.Parse(name);
        return IsImportantAoCSClass(classSymbol, parsed);
    }
    public static bool IsImportantAoCSClass(INamedTypeSymbol classSymbol, IdentifierWithArity name)
    {
        return IsImportantAoCSClass(classSymbol, ExpectedSymbolName());

        FullSymbolName ExpectedSymbolName() => new(name, new[] { nameof(AdventOfCSharp) });
    }

    public static bool IsImportantAoCSClass(INamedTypeSymbol classSymbol, FullSymbolName name)
    {
        return classSymbol.GetAllBaseTypesAndInterfaces().Any(Matches);

        bool Matches(INamedTypeSymbol baseType)
        {
            var fullBaseName = baseType.GetFullSymbolName(SymbolNameKind.Normal)!;
            return fullBaseName.Matches(name, SymbolNameMatchingLevel.Namespace);
        }
    }
}
