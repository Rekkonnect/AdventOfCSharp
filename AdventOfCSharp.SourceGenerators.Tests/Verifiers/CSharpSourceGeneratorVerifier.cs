using AdventOfCSharp.AnalysisTestsBase.Verifiers;
using AdventOfCSharp.SourceGenerators.Tests.Helpers;
using AdventOfCSharp.Testing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RoseLynn.Generators.Testing;

namespace AdventOfCSharp.SourceGenerators.Tests.Verifiers;

public static class CSharpSourceGeneratorVerifier<TSourceGenerator>
    where TSourceGenerator : ISourceGenerator, new()
{
    public class Test : CSharpSourceGeneratorTestEx<TSourceGenerator, NUnitVerifier>
    {
        // Expose it like that
        public string AssemblyName => DefaultTestProjectName;

        public Test()
        {
            CSharpVerifierHelper.SetupNET6AndAoCSDependencies(this);
            TestState.AdditionalReferences.AddRange(BenchmarkSpecificMetadataReferences.BaseBenchmarkReferences);
            TestState.AdditionalReferences.AddRange(TestingSpecificMetadataReferences.BaseTestingReferences);
        }

        public void AddFrameworkReference(TestingFramework framework)
        {
            TestState.AdditionalReferences.AddRange(TestingSpecificMetadataReferences.ForFramework(framework).References);
        }
    }
}
