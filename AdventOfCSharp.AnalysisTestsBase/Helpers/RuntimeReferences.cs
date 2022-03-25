using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using RoseLynn;
using System.IO;
using System.Reflection;

namespace AdventOfCSharp.AnalysisTestsBase.Helpers;

public static class RuntimeReferences
{
    public static readonly ReferenceAssemblies NET6_0Reference = new(
            "net6.0",
            new PackageIdentity(
                "Microsoft.NETCore.App.Ref", "6.0.0"),
                Path.Combine("ref", "net6.0"));

    public static readonly MetadataReference NETStandard2_0MetadataReference = MetadataReferenceFactory.CreateFromType<Binder>();
}
