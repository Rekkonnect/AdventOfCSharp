using AdventOfCSharp.AnalysisTestsBase.Resources;
using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Collections.Immutable;

namespace AdventOfCSharp.AnalysisTestsBase.Helpers;

public static class AoCSMetadataReferences
{
    public static readonly ImmutableArray<MetadataReference> BaseReferences = ImmutableArray.Create(new MetadataReference[]
    {
        MetadataReferenceFactory.CreateFromType<Problem>(),
        MetadataReferenceFactory.CreateFromType<ProblemDate>(),
        MetadataReferenceFactory.CreateFromType<ExampleAttribute>(),
    });
}
