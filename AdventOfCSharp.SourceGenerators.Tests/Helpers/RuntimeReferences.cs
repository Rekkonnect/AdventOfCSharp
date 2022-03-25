using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Immutable;
using System.Threading;

namespace AdventOfCSharp.SourceGenerators.Tests.Helpers;

public static class RuntimeReferences
{
    public static readonly ImmutableArray<MetadataReference> NET6_0;

    static RuntimeReferences()
    {
        NET6_0 = ReferenceAssemblies.Net.Net60.ResolveAsync(LanguageNames.CSharp, CancellationToken.None).Result;
    }
}
