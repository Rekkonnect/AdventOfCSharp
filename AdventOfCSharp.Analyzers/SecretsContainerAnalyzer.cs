using AdventOfCSharp.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn.CSharp.Syntax;

namespace AdventOfCSharp.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SecretsContainerAnalyzer : AoCSAnalyzer
{
    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterTargetAttributeSyntaxNodeAction(AnalyzeSecretsContainerType, KnownSymbolNames.SecretsContainerAttribute);
    }

    // In an AoC project, attributes should not be too commonly used, meaning less nodes to iterate
    // An average solution project would contain loads of functions, causing far more invocations than necessary
    private void AnalyzeSecretsContainerType(SyntaxNodeAnalysisContext context)
    {
        var attributeNode = context.Node as AttributeSyntax;

        if (attributeNode!.GetParentAttributeList().Parent is not ClassDeclarationSyntax classDeclarationNode)
            return;

        var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationNode)!;

        if (IsValidSecretsContainer(classSymbol))
            return;

        context.ReportDiagnostic(Diagnostics.CreateAoCS0008(attributeNode));
    }

    private static bool IsValidSecretsContainer(INamedTypeSymbol classSymbol)
    {
        return MeetsDeclarationCriteria(classSymbol);

        static bool MeetsDeclarationCriteria(INamedTypeSymbol classSymbol)
        {
            return classSymbol is
                   {
                       IsStatic: false,
                       IsSealed: true,
                   }
                && IsImportantAoCSClass(classSymbol, KnownSymbolNames.ISecretsContainer)
                && classSymbol.HasPublicParameterlessInstanceConstructor();
        }
    }
}
