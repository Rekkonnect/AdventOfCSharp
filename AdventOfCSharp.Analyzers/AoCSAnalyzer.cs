using AdventOfCSharp.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using RoseLynn.Analyzers;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

#nullable enable

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public abstract class AoCSAnalyzer : CSharpDiagnosticAnalyzer
{
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => AoCSDiagnosticDescriptorStorage.Instance;

    public sealed override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        RegisterAnalyzers(context);
    }
    protected abstract void RegisterAnalyzers(AnalysisContext context);

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
        // Analyzing symbols using hard-coded names be like
        if (classSymbol.IsAbstract)
            return false;
        return classSymbol.GetAllBaseTypes().Any(baseType => baseType.FullMetadataName() is $"{nameof(AdventOfCSharp)}.Problem");
    }
}
