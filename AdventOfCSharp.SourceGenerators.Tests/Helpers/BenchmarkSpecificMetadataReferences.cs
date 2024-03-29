﻿using AdventOfCSharp.AnalysisTestsBase.Helpers;
using AdventOfCSharp.Benchmarking;
using Microsoft.CodeAnalysis;
using RoseLynn;
using System;
using System.Collections.Immutable;

namespace AdventOfCSharp.SourceGenerators.Tests.Helpers;

public static class BenchmarkSpecificMetadataReferences
{
    public static readonly MetadataReference NET6_0MetadataReference;

    public static readonly ImmutableArray<MetadataReference> BaseBenchmarkReferences;
    public static readonly ImmutableArray<MetadataReference> AllBaseReferences;
    public static readonly ImmutableArray<MetadataReference> AllBaseReferencesWithRuntime;

    static BenchmarkSpecificMetadataReferences()
    {
        NET6_0MetadataReference = MetadataReferenceFactory.CreateFromType<Attribute>();

        BaseBenchmarkReferences = ImmutableArray.Create(new MetadataReference[]
        {
            // AdventOfCSharp.Benchmarking
            MetadataReferenceFactory.CreateFromType<BenchmarkDescriberHelpers>(),

            // AdventOfCSharp.Benchmarking.Common
            MetadataReferenceFactory.CreateFromType<BenchmarkDescriberAttribute>(),

            MetadataReferenceFactory.CreateFromType<BenchmarkDotNet.Attributes.BenchmarkAttribute>(),
        });

        AllBaseReferences = BaseBenchmarkReferences.AddRange(AoCSMetadataReferences.BaseReferences);

        AllBaseReferencesWithRuntime = AllBaseReferences.AddRange(RuntimeReferences.NET6_0);
    }
}
