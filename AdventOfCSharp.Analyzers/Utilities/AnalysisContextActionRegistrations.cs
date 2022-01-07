using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;

namespace AdventOfCSharp.Analyzers.Utilities;

#nullable enable

public static class AnalysisContextActionRegistrations
{
    public static void RegisterTargetAttributeSyntaxNodeAction(this AnalysisContext context, Action<SyntaxNodeAnalysisContext> action, string attributeName)
    {
        context.RegisterSyntaxNodeAction(Boilerplate, SyntaxKind.Attribute);

        void Boilerplate(SyntaxNodeAnalysisContext context)
        {
            var attributeNode = (context.Node as AttributeSyntax)!;

            var attributeTypeSymbol = context.SemanticModel.GetAttributeTypeSymbol(attributeNode);
            if (attributeTypeSymbol.Name != attributeName)
                return;

            action(context);
        }
    }
}
