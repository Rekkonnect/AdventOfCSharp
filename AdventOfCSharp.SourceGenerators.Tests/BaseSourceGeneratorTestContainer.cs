using AdventOfCSharp.SourceGenerators.Tests.Helpers;
using AdventOfCSharp.SourceGenerators.Tests.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventOfCSharp.SourceGenerators.Tests;

public abstract class BaseSourceGeneratorTestContainer<TSourceGenerator>
    where TSourceGenerator : class, ISourceGenerator, new()
{
    protected Compilation CreateCompilationRunGenerator(string source, out TSourceGenerator generator, out GeneratorDriver resultingGeneratorDriver, out Compilation initialCompilation)
    {
        return CreateCompilationRunGenerator(new[] { source }, out generator, out resultingGeneratorDriver, out initialCompilation);
    }
    protected Compilation CreateCompilationRunGenerator(IEnumerable<string> sources, out TSourceGenerator generator, out GeneratorDriver resultingGeneratorDriver, out Compilation initialCompilation)
    {
        var references = BenchmarkSpecificMetadataReferences.AllBaseReferences;
        var trees = sources.Select(source => CSharpSyntaxTree.ParseText(source));
        initialCompilation = CSharpCompilation.Create(null, trees, references);
        generator = new();
        var driver = CSharpGeneratorDriver.Create(generator);
        resultingGeneratorDriver = driver.RunGeneratorsAndUpdateCompilation(initialCompilation, out var resultCompilation, out _);
        return resultCompilation;
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

    protected sealed class GeneratedSourceMappings : SortedDictionary<string, string> { }
}
