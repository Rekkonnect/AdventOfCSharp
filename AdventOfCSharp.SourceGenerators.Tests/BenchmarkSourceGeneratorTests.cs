using NUnit.Framework;

namespace AdventOfCSharp.SourceGenerators.Tests;

public sealed class BenchmarkSourceGeneratorTests : BaseSourceGeneratorTestContainer<BenchmarkSourceGenerator>
{
    protected override BenchmarkSourceGenerator InitializeGeneratorInstance() => new();

    [Test]
    public void SingleNamespaceMultipleDaysTest()
    {
        const string baseNamespace = "AoC.Test";

        const string problemClasses =
$@"
using AdventOfCSharp;

namespace {baseNamespace}.Year2021;

public sealed class Day1 : TestProblem {{ }}
public sealed class Day3 : TestProblem {{ }}
public sealed class Day4 : TestProblem {{ }}
public sealed class Day6 : TestProblem {{ }}

public abstract class TestProblem : Problem<int>
{{
    public sealed override int SolvePart1() => -1;
    public sealed override int SolvePart2() => -2;
}}
";

        var mappings = new GeneratedSourceMappings();
        AddMapping(2021, 1);
        AddMapping(2021, 3);
        AddMapping(2021, 4);
        AddMapping(2021, 6);

        VerifyAsync(problemClasses, mappings).Wait();

        void AddMapping(int year, int day)
        {
            var source = BenchmarkSourceGenerator.GenerateBenchmarkSourceFromBaseNamespace(year, day, baseNamespace);
            var sourceName = BenchmarkSourceGenerator.GetBenchmarkSourceFileName(year, day);

            mappings.Add(sourceName, source);
        }
    }
}
