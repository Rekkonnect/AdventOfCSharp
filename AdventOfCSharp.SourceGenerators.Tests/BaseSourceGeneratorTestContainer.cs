using AdventOfCSharp.SourceGenerators.Tests.Helpers;
using AdventOfCSharp.SourceGenerators.Tests.Verifiers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
        var references = BenchmarkSpecificMetadataReferences.AllBaseReferencesWithRuntime;
        var parseOptions = new CSharpParseOptions(LanguageVersion.LatestMajor);
        var trees = sources.Select(source => CSharpSyntaxTree.ParseText(source, options: parseOptions));
        var options = new CSharpCompilationOptions(OutputKind.NetModule);
        initialCompilation = CSharpCompilation.Create(null, trees, references, options);

        var diagnostics = initialCompilation.GetDiagnostics();
        Debug.Assert(diagnostics.IsEmpty);

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
