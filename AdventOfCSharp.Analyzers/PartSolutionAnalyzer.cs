using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;

namespace AdventOfCSharp.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PartSolutionAnalyzer : ProblemAoCSAnalyzer
{
    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeSolutionMethodDeclaration, SymbolKind.Method);
        context.RegisterSymbolAction(AnalyzePartSolutionAttributeUsage, SymbolKind.Method);
    }

    private void AnalyzeSolutionMethodDeclaration(SymbolAnalysisContext context)
    {
        switch (context.Symbol)
        {
            case IMethodSymbol methodSymbol:
                // If only I could use dependencies
                if (!methodSymbol.IsStatic)
                {
                    bool isPartSolutionMethod = methodSymbol.HasInheritedAttributeNamed(KnownSymbolNames.PartSolverAttribute);
                    if (isPartSolutionMethod)
                        return;
                }

                var solutionAttribute = GetPartSolutionAttributeData(methodSymbol);
                if (solutionAttribute is null)
                    return;

                var solutionAttributeNode = solutionAttribute.ApplicationSyntaxReference.GetSyntax() as AttributeSyntax;
                context.ReportDiagnostic(Diagnostics.CreateAoCS0001(solutionAttributeNode));
                break;
        }
    }

    private void AnalyzePartSolutionAttributeUsage(SymbolAnalysisContext context)
    {
        switch (context.Symbol)
        {
            case IMethodSymbol methodSymbol:
                var solutionAttribute = GetPartSolutionAttributeData(methodSymbol);
                if (solutionAttribute?.ConstructorArguments.Length is null or 0)
                    return;

                var argument = solutionAttribute.ConstructorArguments[0];
                if (IsDefinedEnumValue(argument))
                    return;

                var attributeNode = solutionAttribute.ApplicationSyntaxReference.GetSyntax() as AttributeSyntax;
                var undefinedEnumValueNode = attributeNode.ArgumentList.Arguments[0];
                context.ReportDiagnostic(Diagnostics.CreateAoCS0002(undefinedEnumValueNode));

                break;
        }
    }

    private static AttributeData? GetPartSolutionAttributeData(IMethodSymbol method)
    {
        return method.FirstOrDefaultAttributeNamed(KnownSymbolNames.PartSolutionAttribute);
    }

    private static bool IsDefinedEnumValue(TypedConstant constant)
    {
        var enumSymbol = constant.Type as INamedTypeSymbol;
        if (enumSymbol?.TypeKind != TypeKind.Enum)
            return false;

        var definedEnumValues = enumSymbol.GetMembers().OfType<IFieldSymbol>();
        foreach (var value in definedEnumValues)
        {
            if (value.ConstantValue!.Equals(constant.Value))
                return true;
        }
        return false;
    }
}
