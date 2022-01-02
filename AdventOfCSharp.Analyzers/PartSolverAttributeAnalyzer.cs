using AdventOfCSharp.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis;
using RoseLynn;
using RoseLynn.CSharp.Syntax;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

#nullable enable

public sealed class PartSolverAttributeAnalyzer : AoCSAnalyzer
{
    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzePartSolverAttributeValidity, SyntaxKind.Attribute);
    }

    // In an AoC project, attributes should not be too commonly used, meaning less nodes to iterate
    // An average solution project would contain loads of functions, causing far more invocations than necessary
    private void AnalyzePartSolverAttributeValidity(SyntaxNodeAnalysisContext context)
    {
        var attributeNode = context.Node as AttributeSyntax;
        if (attributeNode!.GetParentAttributeList().Parent is not MethodDeclarationSyntax methodDeclarationNode)
            return;

        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationNode)!;

        if (!methodSymbol.HasAttributeNamed(KnownSymbolNames.PartSolverAttribute))
            return;

        if (IsValidPartSolver(methodSymbol))
            return;

        var solverAttribute = methodSymbol.FirstOrDefaultAttribute(KnownSymbolNames.PartSolverAttribute)!;
        var solverAttributeNode = solverAttribute.ApplicationSyntaxReference!.GetSyntax() as AttributeSyntax;
        context.ReportDiagnostic(Diagnostics.CreateAoCS0013(solverAttributeNode));
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
            return methodSymbol is
                   {
                       DeclaredAccessibility: Accessibility.Public,
                       IsStatic: false,
                   }
                && methodSymbol.ReturnType.SpecialType is not SpecialType.System_Void
                && methodSymbol.IsParameterlessNonGenericMethod();
        }
    }
}
