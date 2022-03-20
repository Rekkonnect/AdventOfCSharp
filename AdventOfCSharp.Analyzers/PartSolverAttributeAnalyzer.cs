using AdventOfCSharp.CodeAnalysis.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using RoseLynn.CSharp;
using RoseLynn.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

#nullable enable

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PartSolverAttributeAnalyzer : ProblemAoCSAnalyzer
{
    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterTargetAttributeSyntaxNodeAction(AnalyzePartSolverAttribute, nameof(PartSolverAttribute));
    }

    private void AnalyzePartSolverAttribute(SyntaxNodeAnalysisContext context)
    {
        AnalyzePartSolverAttributeValidity(context);
        AnalyzePartName(context);
    }

    private void AnalyzePartName(SyntaxNodeAnalysisContext context)
    {
        var partSolverAttributeNode = context.Node as AttributeSyntax;

        var partSolverAttributeData = partSolverAttributeNode!.GetAttributeData(context.SemanticModel, out var attributedSymbol)!;
        var arguments = partSolverAttributeData.ConstructorArguments;
        if (arguments.Length < 1)
            return;

        if (arguments[0].Value is not string partName)
            return;

        var partNameArgumentNode = partSolverAttributeNode!.ArgumentList!.Arguments[0];
        if (partName.Length > 20)
        {
            context.ReportDiagnostic(Diagnostics.CreateAoCS0015(partNameArgumentNode));
        }

        var otherPartSolverAttributes = GetAllPartSolverAttributesForClass(attributedSymbol!.ContainingType)
                                       .Except(new[] { partSolverAttributeData });

        var names = PartNames(otherPartSolverAttributes);
        if (names.Contains(partName))
        {
            context.ReportDiagnostic(Diagnostics.CreateAoCS0014(partNameArgumentNode));
        }

        static IEnumerable<string> PartNames(IEnumerable<AttributeData> attributes) 
        {
            return attributes.Select(attribute => attribute.ConstructorArguments[0].Value as string) as IEnumerable<string>;
        }
    }

    private static IEnumerable<AttributeData> GetAllPartSolverAttributesForClass(INamedTypeSymbol problemSolutionClassSymbol)
    {
        var methods = problemSolutionClassSymbol.GetAllMembersIncludingInherited().OfType<IMethodSymbol>();
        return methods.Select(GetPartSolverAttribute).Where(data => data is not null) as IEnumerable<AttributeData>;

        static AttributeData? GetPartSolverAttribute(IMethodSymbol method)
        {
            return method.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass!.Name is nameof(PartSolverAttribute));
        }
    }

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
