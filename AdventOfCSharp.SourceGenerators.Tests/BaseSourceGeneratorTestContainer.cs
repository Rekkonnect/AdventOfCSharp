using AdventOfCSharp.SourceGenerators.Tests.Helpers;
using AdventOfCSharp.SourceGenerators.Tests.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using NUnit.Framework;
using RoseLynn.Generators;
using RoseLynn.Testing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdventOfCSharp.SourceGenerators.Tests;

public abstract class BaseSourceGeneratorTestContainer<TSourceGenerator>
    : BaseSourceGeneratorTestContainer<TSourceGenerator, NUnitVerifier, CSharpSourceGeneratorVerifier<TSourceGenerator>.Test>
    where TSourceGenerator : class, ISourceGenerator, new()
{
    protected override LanguageVersion LanguageVersion => LanguageVersion.LatestMajor;
    protected override IEnumerable<MetadataReference> DefaultMetadataReferences => BenchmarkSpecificMetadataReferences.AllBaseReferencesWithRuntime;

    protected Compilation CreateCompilationRunGenerator(string source, out TSourceGenerator generator, out GeneratorDriver resultingGeneratorDriver, out Compilation initialCompilation)
    {
        return CreateCompilationRunGenerator(new[] { source }, out generator, out resultingGeneratorDriver, out initialCompilation);
    }
    protected Compilation CreateCompilationRunGenerator(IEnumerable<string> sources, out TSourceGenerator generator, out GeneratorDriver resultingGeneratorDriver, out Compilation initialCompilation)
    {
        var resultingCompilation = base.CreateCompilationRunGenerator(sources, out generator, out resultingGeneratorDriver, out initialCompilation);

        var diagnostics = initialCompilation.GetDiagnostics();
        Assert.That(diagnostics.IsEmpty, Is.True);

        return resultingCompilation;
    }

    protected async Task VerifyAsync(string source, string generatedFileName, string generatedSource)
    {
        var mappings = new GeneratedSourceMappings()
        {
            { generatedFileName, generatedSource }
        };
        await VerifyAsync(source, mappings);
    }
    protected async Task VerifyAsync(string source, GeneratedSourceMappings mappings)
    {
        await VerifyAsync(new[] { source }, mappings);
    }
    protected async Task VerifyAsync(string source, GeneratedSourceMappings mappings, CSharpSourceGeneratorVerifier<TSourceGenerator>.Test test)
    {
        await VerifyAsync(new[] { source }, mappings, test);
    }
    protected async Task VerifyAsync(IEnumerable<string> sources, GeneratedSourceMappings mappings)
    {
        await VerifyAsync(sources, mappings, new CSharpSourceGeneratorVerifier<TSourceGenerator>.Test());
    }
    protected async Task VerifyAsync(IEnumerable<string> sources, GeneratedSourceMappings mappings, CSharpSourceGeneratorVerifier<TSourceGenerator>.Test test)
    {
        test.TestState.Sources.AddRange(sources);
        foreach (var mapping in mappings)
        {
            test.TestState.GeneratedSources.Add((typeof(TSourceGenerator), mapping.Key, mapping.Value));
        }

        await test.RunAsync();
    }
}
