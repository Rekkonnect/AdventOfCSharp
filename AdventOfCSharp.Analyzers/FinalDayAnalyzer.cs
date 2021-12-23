using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

public sealed class FinalDayAnalyzer : AoCSAnalyzer
{
    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeProblemSolutionType, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeProblemSolutionType(SyntaxNodeAnalysisContext context)
    {

    }
}
