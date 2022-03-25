using AdventOfCSharp.AnalysisTestsBase.Resources;
using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Collections.Generic;

namespace AdventOfCSharp.AnalysisTestsBase.Helpers;

public static class AoCSMetadataReferences
{
    public static IEnumerable<MetadataReference> CreateBaseMetadataReferences() => new[]
    {
        MetadataReferenceFactory.CreateFromType<Problem>(),
        MetadataReferenceFactory.CreateFromType<ExampleAttribute>(),
    };
}
