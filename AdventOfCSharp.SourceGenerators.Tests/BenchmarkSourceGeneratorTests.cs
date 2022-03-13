using AdventOfCSharp.SourceGenerators.Tests.Verifiers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.SourceGenerators.Tests;
[TestClass]
public sealed class BenchmarkSourceGeneratorTests : BaseSourceGeneratorTestContainer<BenchmarkSourceGenerator>
{
    public override BenchmarkSourceGenerator InitializeGeneratorInstance() => new();

    [TestMethod]
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

        //var compilation = CreateCompilation(problemClasses, out var driver);
        //var trees = compilation.SyntaxTrees.ToArray();
        //Assert.AreEqual(2, trees.Length);

        //AssertGeneratedForDay(1);
        //AssertGeneratedForDay(3);
        //AssertGeneratedForDay(4);
        //AssertGeneratedForDay(6);

        //void AssertGeneratedForDay(int day)
        //{
        //    Assert.AreEqual(2, compilation.GlobalNamespace.GetTypeMembers($"Day{day}").Length);
        //}

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
