using AdventOfCSharp.CodeFixes.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using static AdventOfCSharp.Analyzers.AoCSDiagnosticDescriptorStorage;

namespace AdventOfCSharp.CodeFixes;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ProblemClassSimplifier))]
public sealed class ProblemClassSimplifier : AoCSCodeFixProvider
{
    protected override IEnumerable<DiagnosticDescriptor> FixableDiagnosticDescriptors => new DiagnosticDescriptor[]
    {
        Instance[0003]
    };

    protected override async Task<Document> PerformCodeFixActionAsync(CodeFixContext context, SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        return await context.Document.RemoveText(context.Diagnostics[0].Location, cancellationToken);
    }
}
