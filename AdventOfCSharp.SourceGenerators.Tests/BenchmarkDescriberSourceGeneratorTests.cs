using AdventOfCSharp.Benchmarking;
using AdventOfCSharp.SourceGenerators.Utilities;
using NUnit.Framework;
using System.Collections.Generic;

namespace AdventOfCSharp.SourceGenerators.Tests;

public sealed class BenchmarkDescriberSourceGeneratorTests : BaseSourceGeneratorTestContainer<BenchmarkDescriberSourceGenerator>
{
    private const string baseProblemNamespace = "AoC.Test";

    [Test]
    public void ConsumerBenchmarkGenerationTest()
    {
        const string baseDescriberNamespace = "AoC.BenchmarkDescribers";
        const string describerName = "Consumer";

        const string benchmarkDescriberSource =
$@"
using AdventOfCSharp;
using AdventOfCSharp.Benchmarking;

namespace {baseProblemNamespace}.Year2021
{{
    public sealed class Day1 : TestProblem {{ }}

    public abstract class TestProblem : Problem<int>
    {{
        public sealed override int SolvePart1() => -1;
        public sealed override int SolvePart2() => -2;
    }}
}}

namespace {baseDescriberNamespace}
{{
    [BenchmarkDescriber]
    [AllDates]
    public sealed partial class {describerName}
    {{
    }}
}}
";

        const string describerImplementationSource =
$@"
using AdventOfCSharp;
using AdventOfCSharp.Benchmarking;
using BenchmarkDotNet.Attributes;
using System;

using static AdventOfCSharp.Benchmarking.BenchmarkDescriberHelpers;

#nullable disable

namespace {baseDescriberNamespace}
{{
    partial class {describerName}
    {{
        private readonly Problem year2021day1 = Problem.CreateNewLoadedState<{baseProblemNamespace}.Year2021.Day1>();
        private Action year2021day1part1, year2021day1part2, year2021day1input;

        [GlobalSetup]
        public void Setup()
        {{
            SetupActions();
        }}

        private void SetupActions()
        {{
            CreateAssignBenchmarkedActions(year2021day1, ref year2021day1part1, ref year2021day1part2, ref year2021day1input);
        }}

        [Benchmark]
        [BenchmarkCategory(""Year 2021 Day 01"")]
        public void Year2021_Day01_Part1()
        {{
            year2021day1part1();
        }}
        [Benchmark]
        [BenchmarkCategory(""Year 2021 Day 01"")]
        public void Year2021_Day01_Part2()
        {{
            year2021day1part2();
        }}
    }}
}}
";

        var mappings = new GeneratedSourceMappings();
        AddDescriberMapping(mappings, describerName, baseDescriberNamespace, describerImplementationSource);

        VerifyAsync(benchmarkDescriberSource, mappings).Wait();
    }

    [TestCase(PartSolutionStatus.Uninitialized)]
    [TestCase(PartSolutionStatus.WIP)]
    [TestCase(PartSolutionStatus.UnavailableLockedStar)]
    [TestCase(PartSolutionStatus.Interactive)]
    [TestCase(PartSolutionStatus.Refactoring)]
    public void InvalidSolutionBenchmarkGenerationTest(PartSolutionStatus invalidPartSolutionStatus)
    {
        const string baseDescriberNamespace = "AoC.BenchmarkDescribers";
        const string describerName = "Consumer";

        string benchmarkDescriberSource =
$@"
using AdventOfCSharp;
using AdventOfCSharp.Benchmarking;

namespace {baseProblemNamespace}.Year2021
{{
    public sealed class Day1 : Problem<int>
    {{
        [PartSolution(PartSolutionStatus.{invalidPartSolutionStatus})]
        public sealed override int SolvePart1() => -1;
        public sealed override int SolvePart2() => -2;
    }}
    public sealed class Day2 : Problem<int>
    {{
        public sealed override int SolvePart1() => -1;
        [PartSolution(PartSolutionStatus.{invalidPartSolutionStatus})]
        public sealed override int SolvePart2() => -2;
    }}
}}

namespace {baseDescriberNamespace}
{{
    [BenchmarkDescriber]
    [AllDates]
    public sealed partial class {describerName}
    {{
    }}
}}
";

        const string describerImplementationSource =
$@"
using AdventOfCSharp;
using AdventOfCSharp.Benchmarking;
using BenchmarkDotNet.Attributes;
using System;

using static AdventOfCSharp.Benchmarking.BenchmarkDescriberHelpers;

#nullable disable

namespace {baseDescriberNamespace}
{{
    partial class {describerName}
    {{
        private readonly Problem year2021day1 = Problem.CreateNewLoadedState<{baseProblemNamespace}.Year2021.Day1>();
        private Action year2021day1part1, year2021day1part2, year2021day1input;
        private readonly Problem year2021day2 = Problem.CreateNewLoadedState<{baseProblemNamespace}.Year2021.Day2>();
        private Action year2021day2part1, year2021day2part2, year2021day2input;

        [GlobalSetup]
        public void Setup()
        {{
            SetupActions();
        }}

        private void SetupActions()
        {{
            CreateAssignBenchmarkedActions(year2021day1, ref year2021day1part1, ref year2021day1part2, ref year2021day1input);
            CreateAssignBenchmarkedActions(year2021day2, ref year2021day2part1, ref year2021day2part2, ref year2021day2input);
        }}

        [Benchmark]
        [BenchmarkCategory(""Year 2021 Day 01"")]
        public void Year2021_Day01_Part2()
        {{
            year2021day1part2();
        }}
        [Benchmark]
        [BenchmarkCategory(""Year 2021 Day 02"")]
        public void Year2021_Day02_Part1()
        {{
            year2021day2part1();
        }}
    }}
}}
";

        var mappings = new GeneratedSourceMappings();
        AddDescriberMapping(mappings, describerName, baseDescriberNamespace, describerImplementationSource);

        VerifyAsync(benchmarkDescriberSource, mappings).Wait();
    }

    private const string problemClasses =
$@"
using AdventOfCSharp;

namespace {baseProblemNamespace}.Year2019
{{
    public sealed class Day1 : TestProblem {{ }}
}}
namespace {baseProblemNamespace}.Year2020
{{
    public sealed class Day1 : TestProblem {{ }}
    public sealed class Day2 : TestProblem {{ }}
    public sealed class Day3 : TestProblem {{ }}
}}
namespace {baseProblemNamespace}.Year2021
{{
    public sealed class Day1 : TestProblem {{ }}
    public sealed class Day2 : TestProblem {{ }}
}}

public abstract class TestProblem : Problem<int>
{{
    public sealed override int SolvePart1() => -1;
    public sealed override int SolvePart2() => -2;
}}
";

    [Test]
    public void AllDatesDescriberTest()
    {
        const string attributes =
@"
[AllDates]
";
        var expectedDates = new ProblemDate[]
        {
            new(2019, 1),

            new(2020, 1),
            new(2020, 2),
            new(2020, 3),

            new(2021, 1),
            new(2021, 2),
        };
        DescriberTest(attributes, expectedDates);
    }
    [Test]
    public void SelectedYearsDescriberTest()
    {
        const string attributes =
@"
[Years(2021, 2019, 2015)]
";
        var expectedDates = new ProblemDate[]
        {
            new(2019, 1),

            new(2021, 1),
            new(2021, 2),
        };
        DescriberTest(attributes, expectedDates);
    }
    [Test]
    public void SelectedDaysDescriberTest()
    {
        const string attributes =
@"
[Days(2, 3, 10)]
";
        var expectedDates = new ProblemDate[]
        {
            new(2020, 2),
            new(2020, 3),

            new(2021, 2),
        };
        DescriberTest(attributes, expectedDates);
    }
    [Test]
    public void SelectedDatesDescriberTest()
    {
        const string attributes =
@"
[Dates(2021, 1, 2, 5, 12)]
";
        var expectedDates = new ProblemDate[]
        {
            new(2021, 1),
            new(2021, 2),
        };
        DescriberTest(attributes, expectedDates);
    }
    [Test]
    public void CombinedYearsDatesDescriberTest()
    {
        const string attributes =
@"
[Years(2019)]
[Dates(2020, 3)]
[Dates(2021, 2)]
";
        var expectedDates = new ProblemDate[]
        {
            new(2019, 1),

            new(2020, 3),

            new(2021, 2),
        };
        DescriberTest(attributes, expectedDates);
    }
    [Test]
    public void ConstrainedPartsDescriberTest()
    {
        const string attributes =
@"
[Years(2019)]
[Parts(BenchmarkingParts.Part1)]
";

        var expectedDates = new ProblemDate[]
        {
            new(2019, 1),
        };
        DescriberTest(attributes, expectedDates, BenchmarkingParts.Part1);
    }

    private void DescriberTest(string attributes, IEnumerable<ProblemDate> expectedDates, BenchmarkingParts expectedParts = BenchmarkingParts.OnlyParts)
    {
        const string baseDescriberNamespace = "AoC.BenchmarkDescribers";
        const string describerName = "Consumer";

        string benchmarkDescriberSource =
$@"
using AdventOfCSharp.Benchmarking;

namespace {baseDescriberNamespace};

[BenchmarkDescriber]
{attributes}
public sealed partial class {describerName}
{{
}}
";

        var sources = new[] { problemClasses, benchmarkDescriberSource };
        var compilation = CreateCompilationRunGenerator(sources, out var generator, out var driver, out var initialCompilation);

        var results = generator.GetExecutionResults(initialCompilation);
        var benchmarkSymbol = initialCompilation.Assembly.GetTypeByMetadataName($"{baseDescriberNamespace}.{describerName}");
        var benchmarkResults = results[benchmarkSymbol];
        CollectionAssert.AreEquivalent(expectedDates, benchmarkResults.Dates);
        Assert.That(benchmarkResults.BenchmarkingParts, Is.EqualTo(expectedParts));
    }

    private static void AddDescriberMapping(GeneratedSourceMappings mappings, string describerName, string baseNamespace, string implementationSource)
    {
        var sourceName = BenchmarkDescriberSourceGenerator.GetBenchmarkSourceFileName(baseNamespace, describerName);

        mappings.Add(sourceName, implementationSource);
    }
}
