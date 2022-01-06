using AdventOfCSharp.Analyzers.Utilities;
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

    protected static bool IsImportantAoCSClass(INamedTypeSymbol classSymbol, string name)
    {
        if (classSymbol.IsAbstract)
            return false;

        return classSymbol.GetAllBaseTypesAndInterfaces().Any(baseType => baseType.FullMetadataName() == $"{nameof(AdventOfCSharp)}.{name}");
    }
}
