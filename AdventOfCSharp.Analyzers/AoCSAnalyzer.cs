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
        var parsed = IdentifierWithArity.Parse(name);
        return IsImportantAoCSClass(classSymbol, parsed);
    }
    protected static bool IsImportantAoCSClass(INamedTypeSymbol classSymbol, IdentifierWithArity name)
    {
        return classSymbol.GetAllBaseTypesAndInterfaces().Any(Matches);

        bool Matches(INamedTypeSymbol baseType)
        {
            var fullBaseName = baseType.GetFullSymbolName(SymbolNameKind.Normal)!;
            return fullBaseName.Matches(ExpectedSymbolName(), SymbolNameMatchingLevel.Namespace);
        }
        FullSymbolName ExpectedSymbolName() => new(name, new[] { nameof(AdventOfCSharp) });
    }
}
