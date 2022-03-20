using AdventOfCSharp.CodeAnalysis.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using RoseLynn.Analyzers;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

#nullable enable

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

    protected static bool IsImportantAoCSClass<T>(INamedTypeSymbol classSymbol)
    {
        return IsImportantAoCSClass(classSymbol, typeof(T).Name);
    }
    protected static bool IsImportantAoCSClass(INamedTypeSymbol classSymbol, string name)
    {
        return AoCSAnalysisHelpers.IsImportantAoCSClass(classSymbol, name);
    }
    protected static bool IsImportantAoCSClass(INamedTypeSymbol classSymbol, IdentifierWithArity name)
    {
        return AoCSAnalysisHelpers.IsImportantAoCSClass(classSymbol, name);
    }
}
