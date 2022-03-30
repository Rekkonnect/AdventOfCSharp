using AdventOfCSharp.Testing;
using Microsoft.CodeAnalysis;
using RoseLynn;
using System;
using System.Collections.Immutable;

namespace AdventOfCSharp.SourceGenerators.Tests.Helpers;

public static class TestingSpecificMetadataReferences
{
    public static readonly TestingFrameworkMetadataReferences NUnitReferences;
    public static readonly TestingFrameworkMetadataReferences XUnitReferences;
    public static readonly TestingFrameworkMetadataReferences MSTestReferences;

    public static readonly MetadataReference TestingCommonReference;
    public static readonly MetadataReference TestingReference;

    public static readonly ImmutableArray<MetadataReference> BaseTestingReferences;

    static TestingSpecificMetadataReferences()
    {
        TestingCommonReference = MetadataReferenceFactory.CreateFromType<AdventOfCSharp.Testing.TestingFrameworkIdentifiers>();
        TestingReference = MetadataReferenceFactory.CreateFromType(typeof(AdventOfCSharp.Testing.FrameworkUnboundProblemValidationTests<>));

        BaseTestingReferences = ImmutableArray.Create(
            TestingCommonReference,
            TestingReference
        );

        var nunitFramework = MetadataReferenceFactory.CreateFromType<NUnit.Framework.Assert>();
        var nunitTesting = MetadataReferenceFactory.CreateFromType(typeof(AdventOfCSharp.Testing.NUnit.NUnitProblemValidationTests<>));
        NUnitReferences = new(nunitTesting, nunitFramework);
        
        var xunitFramework = MetadataReferenceFactory.CreateFromType<Xunit.Assert>();
        var xunitTesting = MetadataReferenceFactory.CreateFromType(typeof(AdventOfCSharp.Testing.XUnit.XUnitProblemValidationTests<>));
        XUnitReferences = new(xunitTesting, xunitFramework);

        var mstestFramework = MetadataReferenceFactory.CreateFromType<Microsoft.VisualStudio.TestTools.UnitTesting.Assert>();
        var mstestTesting = MetadataReferenceFactory.CreateFromType(typeof(AdventOfCSharp.Testing.MSTest.MSTestProblemValidationTests<>));
        MSTestReferences = new(mstestTesting, mstestFramework);
    }

    public static TestingFrameworkMetadataReferences ForFramework(TestingFramework framework)
    {
        return framework switch
        {
            TestingFramework.NUnit => NUnitReferences,
            TestingFramework.XUnit => XUnitReferences,
            TestingFramework.MSTest => MSTestReferences,

            _ => throw new ArgumentException($"Unknown framework: {framework}")
        };
    }

    public sealed class TestingFrameworkMetadataReferences
    {
        public MetadataReference FrameworkReference { get; }
        public MetadataReference AoCSTestingReference { get; }

        public ImmutableArray<MetadataReference> References { get; }

        public TestingFrameworkMetadataReferences(MetadataReference aocsTestingReference, MetadataReference frameworkReference)
        {
            AoCSTestingReference = aocsTestingReference;
            FrameworkReference = frameworkReference;
            References = ImmutableArray.Create(AoCSTestingReference, FrameworkReference);
        }
    }
}
