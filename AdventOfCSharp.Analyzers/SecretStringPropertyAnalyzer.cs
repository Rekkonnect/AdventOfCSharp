using AdventOfCSharp.Analyzers.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoseLynn;
using RoseLynn.CSharp;
using RoseLynn.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCSharp.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SecretStringPropertyAnalyzer : AoCSAnalyzer
{
    protected override void RegisterAnalyzers(AnalysisContext context)
    {
        context.RegisterTargetAttributeSyntaxNodeAction(AnalyzeSecretStringPropertyAttribute, KnownSymbolNames.SecretStringPropertyAttribute);
        context.RegisterSyntaxNodeAction(AnalyzeSecretStringProperty, SyntaxKind.PropertyDeclaration);
        context.RegisterSymbolAction(AnalyzeDeclarationWithoutSecretStringProperties, SymbolKind.NamedType);
    }

    private void AnalyzeDeclarationWithoutSecretStringProperties(SymbolAnalysisContext context)
    {
        var typeSymbol = context.Symbol as INamedTypeSymbol;

        if (typeSymbol.IsAbstract)
            return;

        var isSecretsContainer = IsImportantAoCSClass(typeSymbol, KnownSymbolNames.ISecretsContainer);
        if (!isSecretsContainer)
            return;

        var hasSecretStringProperties = typeSymbol.GetAllMembersIncludingInherited().OfType<IPropertySymbol>().Any(IsSecretStringProperty);

        if (!hasSecretStringProperties)
        {
            var typeDeclarationNode = typeSymbol.DeclaringSyntaxReferences[0].GetSyntax() as TypeDeclarationSyntax;
            context.ReportDiagnostic(Diagnostics.CreateAoCS0081(typeDeclarationNode));
        }
    }

    private void AnalyzeSecretStringPropertyAttribute(SyntaxNodeAnalysisContext context)
    {
        var secretStringPropertyAttributeNode = context.Node as AttributeSyntax;
        var secretTypeArgumentNode = secretStringPropertyAttributeNode.ArgumentList.Arguments[2];
        
        var secretTypeSymbolInfo = context.SemanticModel.GetSymbolInfo(secretTypeArgumentNode.Expression);
        if (secretTypeSymbolInfo.Symbol is IFieldSymbol { IsConst: true })
            return;
        
        context.ReportDiagnostic(Diagnostics.CreateAoCS0082(secretStringPropertyAttributeNode));
    }

    private void AnalyzeSecretStringProperty(SyntaxNodeAnalysisContext context)
    {
        var propertyDeclarationNode = context.Node as PropertyDeclarationSyntax;

        var propertySymbol = context.SemanticModel.GetDeclaredSymbol(propertyDeclarationNode)!;
        if (!IsSecretStringProperty(propertySymbol, out var secretStringAttribute))
            return;

        var containingType = propertySymbol.ContainingType;
        var hasInterface = IsImportantAoCSClass(containingType, KnownSymbolNames.ISecretsContainer);
        if (!hasInterface)
        {
            context.ReportDiagnostic(Diagnostics.CreateAoCS0084(secretStringAttribute));
        }

        if (IsNumerical(propertySymbol.Type.SpecialType))
        {
            context.ReportDiagnostic(Diagnostics.CreateAoCS0083(propertyDeclarationNode));
            return;
        }

        if (secretStringAttribute.ConstructorArguments[0].Value is not string pattern)
            return;

        var regex = new Regex(pattern);

        var returnedExpressions = ReturnedExpressions(propertyDeclarationNode, AccessorKind.Get);
        foreach (var expression in returnedExpressions)
        {
            if (expression is not LiteralExpressionSyntax literal)
                continue;

            if (literal.Token.Value is not string secretString)
                continue;

            if (!MatchesExact(regex, secretString))
                context.ReportDiagnostic(Diagnostics.CreateAoCS0080(literal, secretStringAttribute));
        }
    }

    private static bool IsNumerical(SpecialType specialType)
    {
        return specialType
            is SpecialType.System_Int16
            or SpecialType.System_Int32
            or SpecialType.System_Int64
            or SpecialType.System_UInt16
            or SpecialType.System_UInt32
            or SpecialType.System_UInt64
            or SpecialType.System_Byte
            or SpecialType.System_SByte
            or SpecialType.System_Single
            or SpecialType.System_Double
            or SpecialType.System_Boolean
            or SpecialType.System_Char
            or SpecialType.System_Decimal;
    }

    private static bool IsSecretStringProperty(IPropertySymbol propertySymbol)
    {
        return IsSecretStringProperty(propertySymbol, out _);
    }
    private static bool IsSecretStringProperty(IPropertySymbol propertySymbol, out AttributeData secretStringAttribute)
    {
        while (true)
        {
            secretStringAttribute = propertySymbol.FirstOrDefaultAttributeNamed(KnownSymbolNames.SecretStringPropertyAttribute);
            if (secretStringAttribute is not null)
                return true;

            var overridden = propertySymbol.OverriddenProperty;
            if (overridden is null)
                return false;

            propertySymbol = overridden;
        }
    }

    private static bool MatchesExact(Regex pattern, string s)
    {
        var match = pattern.Match(s);
        return match.Success && match.Length == s.Length;
    }

    private static IEnumerable<ExpressionSyntax> ReturnedExpressions(PropertyDeclarationSyntax property, AccessorKind accessorKind)
    {
        if (property.AccessorList is null)
        {
            if (accessorKind is AccessorKind.Get)
            {
                return new SingleElementCollection<ExpressionSyntax>(property.ExpressionBody.Expression);
            }

            return null;
        }

        return ReturnedExpressions(property.AccessorList.GetAccessor(accessorKind));
    }
    private static IEnumerable<ExpressionSyntax> ReturnedExpressions(AccessorDeclarationSyntax accessor)
    {
        var expressionBody = accessor.ExpressionBody;
        if (expressionBody is not null)
            return new SingleElementCollection<ExpressionSyntax>(expressionBody.Expression);

        // TODO: Filter local methods
        return accessor.DescendantNodes().OfType<ReturnStatementSyntax>().Select(returner => returner.Expression);
    }
}
