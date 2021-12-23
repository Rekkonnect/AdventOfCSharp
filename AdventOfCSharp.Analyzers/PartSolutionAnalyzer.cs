using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

public sealed class PartSolutionAnalyzer : AoCSAnalyzer
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
                    bool isPartSolutionMethod = methodSymbol.HasInheritedAttributeNamed("PartSolverAttribute");
                    if (isPartSolutionMethod)
                        return;
                }

                var solutionAttribute = methodSymbol.GetAttributes().Where(attribute => attribute.AttributeClass.Name == "PartSolutionAttribute").FirstOrDefault();
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
                var solutionAttributes = methodSymbol.GetAttributes().Where(attribute => attribute.AttributeClass.Name == "PartSolutionAttribute");
                foreach (var solutionAttribute in solutionAttributes)
                {
                    if (solutionAttribute.ConstructorArguments.Length is 0)
                        continue;

                    var argument = solutionAttribute.ConstructorArguments[0];
                    if (IsDefinedEnumValue(argument))
                        continue;

                    var attributeNode = solutionAttribute.ApplicationSyntaxReference.GetSyntax() as AttributeSyntax;
                    var undefinedEnumValueNode = attributeNode.ArgumentList.Arguments[0];
                    context.ReportDiagnostic(Diagnostics.CreateAoCS0002(undefinedEnumValueNode));
                }

                break;
        }
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
