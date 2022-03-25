using AdventOfCSharp.CodeAnalysis.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCSharp.Analyzers;

#nullable enable

public abstract class ProblemAoCSAnalyzer : AoCSAnalyzer
{
    protected static bool IsProblemSolutionClass(ClassDeclarationSyntax? classDeclaration, SemanticModel semanticModel)
    {
        return IsProblemSolutionClass(classDeclaration, semanticModel, out _);
    }
    protected static bool IsProblemSolutionClass(ClassDeclarationSyntax? classDeclaration, SemanticModel semanticModel, out INamedTypeSymbol? classSymbol)
    {
        classSymbol = null;
        if (classDeclaration is null)
            return false;
        classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
        return IsProblemSolutionClass(classSymbol!);
    }

    protected static bool IsProblemSolutionClass(INamedTypeSymbol classSymbol)
    {
        return IsImportantAoCSClass(classSymbol, KnownSymbolNames.Problem);
    }
}
