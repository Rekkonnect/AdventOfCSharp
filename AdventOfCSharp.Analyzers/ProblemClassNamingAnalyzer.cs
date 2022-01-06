using AdventOfCSharp.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn.CSharp.Syntax;
using System;
using System.Text.RegularExpressions;

namespace AdventOfCSharp.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ProblemClassNamingAnalyzer : ProblemAoCSAnalyzer
{
    // The patterns do not care about specifics of the valid ranges; those must be handled by the diagnostics
    private static readonly Regex yearPattern = new(@"^Year(?'year'\d*)$", RegexOptions.Compiled);
    private static readonly Regex dayPattern = new(@"^Day(?'day'\d*)$", RegexOptions.Compiled);

    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeNamespaceNaming, SyntaxKind.NamespaceDeclaration, SyntaxKind.FileScopedNamespaceDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeProblemSolutionClassNaming, SyntaxKind.ClassDeclaration);
    }

    // If the analyzer can use the AoC# project dependency,
    // consider abstracting away the year and day validity rules

    private void AnalyzeNamespaceNaming(SyntaxNodeAnalysisContext context)
    {
        var namespaceDeclaration = (context.Node as BaseNamespaceDeclarationSyntax)!;
        var rightmostIdentifier = namespaceDeclaration.Name.GetRightmostIdentifier();
        var match = yearPattern.Match(rightmostIdentifier.Text);
        if (!match.Success)
            return;

        int year = int.Parse(match.Groups["year"].Value);
        if (year < 2015 || year > DateTime.Now.Year)
        {
            context.ReportDiagnostic(Diagnostics.CreateAoCS0005(namespaceDeclaration));
        }
    }

    private void AnalyzeProblemSolutionClassNaming(SyntaxNodeAnalysisContext context)
    {
        AnalyzeDenotedYear(context);
        AnalyzeDenotedDay(context);
    }
    private void AnalyzeDenotedYear(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (context.Node as ClassDeclarationSyntax)!;
        if (!IsProblemSolutionClass(classDeclaration, context.SemanticModel))
            return;

        var identifier = classDeclaration.GetNearestParentOfType<BaseNamespaceDeclarationSyntax>().Name.GetRightmostIdentifier();
        var match = yearPattern.Match(identifier.Text);
        if (!match.Success)
        {
            context.ReportDiagnostic(Diagnostics.CreateAoCS0004(classDeclaration));
        }
    }
    private void AnalyzeDenotedDay(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (context.Node as ClassDeclarationSyntax)!;
        if (!IsProblemSolutionClass(classDeclaration, context.SemanticModel))
            return;

        var identifier = classDeclaration.Identifier;
        var match = dayPattern.Match(identifier.Text);
        if (!match.Success)
        {
            context.ReportDiagnostic(Diagnostics.CreateAoCS0006(classDeclaration));
            return;
        }

        int day = int.Parse(match.Groups["day"].Value);
        if (day < 1 || day > 25)
        {
            context.ReportDiagnostic(Diagnostics.CreateAoCS0007(classDeclaration));
        }
    }

    // This is quite a useful component for RoseLynn
    private class TypeDeclarationInfo
    {
        public BaseTypeDeclarationSyntax DeclarationNode { get; }
        public INamedTypeSymbol DeclaredType { get; }

        public TypeDeclarationInfo(BaseTypeDeclarationSyntax declarationNode, SemanticModel semanticModel)
        {
            DeclarationNode = declarationNode;
            DeclaredType = semanticModel.GetDeclaredSymbol(declarationNode)!;
        }
    }
}
