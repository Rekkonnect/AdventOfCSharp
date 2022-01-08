using AdventOfCSharp.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoseLynn;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static AdventOfCSharp.Analyzers.AoCSDiagnosticDescriptorStorage;

namespace AdventOfCSharp.CodeFixes;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FinalDayUser))]
public sealed class FinalDayUser : AoCSCodeFixProvider
{
    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors => new DiagnosticDescriptor[]
    {
        Instance[0011]
    };

    protected override async Task<Document> PerformCodeFixActionAsync(CodeFixContext context, SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        var baseTypeSyntax = syntaxNode as BaseTypeSyntax;
        var classContainerNode = baseTypeSyntax.Parent.Parent as ClassDeclarationSyntax;
        var semanticModel = await context.Document.GetSemanticModelAsync(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return context.Document;

        var declaredClassSymbol = semanticModel.GetDeclaredSymbol(classContainerNode, cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return context.Document;

        var replaced = await ReplaceInheritedType(context.Document);

        if (cancellationToken.IsCancellationRequested)
            return context.Document;

        var newSemanticModel = await replaced.GetSemanticModelAsync(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return context.Document;

        var newClassSymbols = newSemanticModel.Compilation.GetSymbolsWithName(declaredClassSymbol.Name, SymbolFilter.Type, cancellationToken);
        declaredClassSymbol = newClassSymbols.First() as INamedTypeSymbol;

        return await RemovePart2SolverOverride(replaced);

        async Task<Document> ReplaceInheritedType(Document document)
        {
            var genericBaseType = baseTypeSyntax.Type as GenericNameSyntax;
            var firstArgument = genericBaseType.TypeArgumentList.Arguments[0];
            // TODO: Add ExtendedSyntaxFactory.SeparatedList(params TNode[]) RoseLynn
            var newBaseTypeArgumentList = SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(new[] { firstArgument }));
            var finalDayIdentifier = SyntaxFactory.Identifier(KnownSymbolNames.FinalDay);
            var newBaseTypeSyntax = SyntaxFactory.SimpleBaseType(SyntaxFactory.GenericName(finalDayIdentifier, newBaseTypeArgumentList))
                .WithTriviaFrom(baseTypeSyntax);

            return await document.ReplaceNodeAsync(baseTypeSyntax, newBaseTypeSyntax, cancellationToken);
        }
        async Task<Document> RemovePart2SolverOverride(Document document)
        {
            var member = declaredClassSymbol.GetMembers().First(MatchPart2OverrideMember);
            var declaringSyntax = await member.DeclaringSyntaxReferences.First().GetSyntaxAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return context.Document;

            return await document.RemoveSyntaxNodeAsync(declaringSyntax, SyntaxRemoveOptions.KeepNoTrivia, cancellationToken);

            static bool MatchPart2OverrideMember(ISymbol symbol)
            {
                return symbol is IMethodSymbol
                {
                    Name: KnownSymbolNames.SolvePart2,
                    IsOverride: true,
                    Parameters.IsEmpty: true,
                };
            }
        }
    }
}
