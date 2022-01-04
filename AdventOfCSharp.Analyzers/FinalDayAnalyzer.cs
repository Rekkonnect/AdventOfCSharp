using AdventOfCSharp.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

#nullable enable

public sealed class FinalDayAnalyzer : ProblemAoCSAnalyzer
{
    private const string day25Name = "Day25";
    private const string finalDayClassName = $"{nameof(AdventOfCSharp)}.FinalDay`1";

    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeProblemSolutionType, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeProblemSolutionType(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = context.Node as ClassDeclarationSyntax;
        if (!IsProblemSolutionClass(classDeclaration, context.SemanticModel, out var classSymbol))
            return;

        bool isDay25 = classDeclaration?.Identifier.ValueText == day25Name;

        if (InheritsFinalDayClass(classSymbol!) == isDay25)
            return;

        Diagnostics.FinalDayInheritanceDiagnosticCreator creator = isDay25 switch
        {
            true => Diagnostics.CreateAoCS0011,
            false => Diagnostics.CreateAoCS0012,
        };
        context.ReportDiagnostic(creator(classDeclaration));
    }

    private static bool InheritsFinalDayClass(INamedTypeSymbol classSymbol)
    {
        var baseTypes = classSymbol.GetAllBaseTypes();
        return baseTypes.Any(baseType => baseType.FullMetadataName() is finalDayClassName);
    }
}
