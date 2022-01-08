using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCSharp.CodeFixes.Extensions;

public static class DocumentExtensions
{
    public static async Task<Document> RemoveText(this Document document, Location location, CancellationToken cancellationToken = default)
    {
        return await RemoveText(document, location.SourceSpan, cancellationToken);
    }
    public static async Task<Document> RemoveText(this Document document, TextSpan location, CancellationToken cancellationToken = default)
    {
        var originalText = await document.GetTextAsync(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return document;

        var newText = originalText.Replace(location, "");

        if (cancellationToken.IsCancellationRequested)
            return document;

        return document.WithText(newText);
    }
}
