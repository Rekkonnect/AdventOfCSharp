using AdventOfCSharp.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using System.Linq;

namespace AdventOfCSharp.Analyzers;

public sealed class ProblemInheritanceAnalyzer : AoCSAnalyzer
{
    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeProblemSolutionType, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeProblemSolutionType(SyntaxNodeAnalysisContext context)
    {
        var semanticModel = context.SemanticModel;
        var classDeclarationNode = context.Node as ClassDeclarationSyntax;

        if (classDeclarationNode.BaseList is null)
            return;

        var declaredClass = semanticModel.GetDeclaredSymbol(classDeclarationNode);
        if (declaredClass is null)
            return;

        var baseType = declaredClass.BaseType;
        if (baseType is not INamedTypeSymbol baseNamedType)
            return;

        if (baseNamedType.Name != "Problem")
            return;

        if (baseNamedType.Arity != 2)
            return;

        var arguments = baseNamedType.TypeArguments;
        if (!arguments[0].Equals(arguments[1], SymbolEqualityComparer.Default))
            return;
        
        var baseListTypes = classDeclarationNode.BaseList.Types;
        var baseTypeNode = baseListTypes.First(MatchesBaseType);
        context.ReportDiagnostic(Diagnostics.CreateAoCS0003(baseTypeNode.Type as GenericNameSyntax));

        bool MatchesBaseType(BaseTypeSyntax baseTypeNode)
        {
            return baseType.Equals(semanticModel.GetSymbol<INamedTypeSymbol>(baseTypeNode.Type), SymbolEqualityComparer.Default);
        }
    }
}
