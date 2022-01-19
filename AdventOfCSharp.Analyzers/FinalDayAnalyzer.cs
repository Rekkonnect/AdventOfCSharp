using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AdventOfCSharp.Analyzers;

#nullable enable

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FinalDayAnalyzer : ProblemAoCSAnalyzer
{
    private const string day25Name = "Day25";
    private const string finalDayClassName = "FinalDay`1";
    private const string finalDayFullClassName = $"{nameof(AdventOfCSharp)}.FinalDay`1";

    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeProblemSolutionType, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeProblemSolutionType(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = context.Node as ClassDeclarationSyntax;

        if (!IsProblemSolutionClass(classDeclaration, context.SemanticModel, out var classSymbol))
            return;

        if (classSymbol!.IsAbstract)
            return;

        bool isDay25 = classDeclaration?.Identifier.ValueText is day25Name;

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
        return IsImportantAoCSClass(classSymbol, finalDayClassName);
    }
}
