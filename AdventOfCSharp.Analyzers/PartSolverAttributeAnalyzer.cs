using AdventOfCSharp.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn.CSharp.Syntax;

namespace AdventOfCSharp.Analyzers;

#nullable enable

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PartSolverAttributeAnalyzer : ProblemAoCSAnalyzer
{
    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterTargetAttributeSyntaxNodeAction(AnalyzePartSolverAttributeValidity, KnownSymbolNames.PartSolverAttribute);
    }

    // In an AoC project, attributes should not be too commonly used, meaning less nodes to iterate
    // An average solution project would contain loads of functions, causing far more invocations than necessary
    private void AnalyzePartSolverAttributeValidity(SyntaxNodeAnalysisContext context)
    {
        var attributeNode = context.Node as AttributeSyntax;

        if (attributeNode!.GetParentAttributeList().Parent is not MethodDeclarationSyntax methodDeclarationNode)
            return;

        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationNode)!;

        if (IsValidPartSolver(methodSymbol))
            return;

        context.ReportDiagnostic(Diagnostics.CreateAoCS0013(attributeNode));
    }

    private static bool IsValidPartSolver(IMethodSymbol methodSymbol)
    {
        if (!MeetsDeclarationCriteria(methodSymbol))
            return false;

        var containing = methodSymbol.ContainingSymbol;
        // Local functions can obviously not be part solvers, as they cannot be invoked on demand
        if (containing is not INamedTypeSymbol containingType)
            return false;

        if (!IsProblemSolutionClass(containingType))
            return false;

        return true;

        static bool MeetsDeclarationCriteria(IMethodSymbol methodSymbol)
        {
            // How could pattern matching be improved to support this kinda syntax?
            // Or would the real fix be enabling extension properties?
            return methodSymbol is
                   {
                       DeclaredAccessibility: Accessibility.Public,
                       IsStatic: false,
                       ReturnType.SpecialType: not SpecialType.System_Void
                   }
                && methodSymbol.IsParameterlessNonGenericMethod();
        }
    }
}
