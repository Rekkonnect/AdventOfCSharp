using AdventOfCSharp.SourceGenerators.Tests.Verifiers;
using NUnit.Framework;
using RoseLynn;

namespace AdventOfCSharp.SourceGenerators.Tests;

using VerifyCS = CSharpSourceGeneratorVerifier<BenchmarkSourceGenerator>;

public sealed class BenchmarkSourceGeneratorTests : BaseSourceGeneratorTestContainer<BenchmarkSourceGenerator>
{
    private const string baseProblemNamespace = "AoC.Test";

    [Test]
    public void SingleNamespaceMultipleDaysTest()
    {
        const string problemClasses =
$@"
using AdventOfCSharp;

namespace {baseProblemNamespace}.Year2021;

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
        AddProblemMapping(mappings, 2021, 1);
        AddProblemMapping(mappings, 2021, 3);
        AddProblemMapping(mappings, 2021, 4);
        AddProblemMapping(mappings, 2021, 6);

        VerifyAsync(problemClasses, mappings).Wait();
    }

    [Test]
    public void MultipleNamespacesTest()
    {
        const string problemClasses =
$@"
using AdventOfCSharp;

namespace {baseProblemNamespace}
{{
    namespace Year2020
    {{
        public sealed class Day2 : TestProblem {{ }}
        public sealed class Day10 : TestProblem {{ }}
    }}
    namespace Year2021
    {{
        public sealed class Day1 : TestProblem {{ }}
        public sealed class Day3 : TestProblem {{ }}
        public sealed class Day4 : TestProblem {{ }}
        public sealed class Day6 : TestProblem {{ }}
    }}

    public abstract class TestProblem : Problem<int>
    {{
        public sealed override int SolvePart1() => -1;
        public sealed override int SolvePart2() => -2;
    }}
}}
";

        var mappings = new GeneratedSourceMappings();
        AddProblemMapping(mappings, 2020, 2);
        AddProblemMapping(mappings, 2020, 10);

        AddProblemMapping(mappings, 2021, 1);
        AddProblemMapping(mappings, 2021, 3);
        AddProblemMapping(mappings, 2021, 4);
        AddProblemMapping(mappings, 2021, 6);

        VerifyAsync(problemClasses, mappings).Wait();
    }

    [Test]
    public void OverlappingDaysTest()
    {
        const string otherBaseProblemNamespace = "AoC.Other.Problems";

        const string problemClasses =
$@"
using AdventOfCSharp;

namespace {baseProblemNamespace}
{{
    namespace Year2015
    {{
        public sealed class Day1 : TestProblem {{ }}
        public sealed class Day2 : TestProblem {{ }}
    }}
    namespace Year2016
    {{
        public sealed class Day1 : TestProblem {{ }}
        public sealed class Day3 : TestProblem {{ }}
    }}
}}

namespace {otherBaseProblemNamespace}
{{
    namespace Year2015
    {{
        public sealed class Day3 : TestProblem {{ }}
        public sealed class Day4 : TestProblem {{ }}
    }}
    namespace Year2016
    {{
        public sealed class Day2 : TestProblem {{ }}
        public sealed class Day4 : TestProblem {{ }}
    }}
}}

public abstract class TestProblem : Problem<int>
{{
    public sealed override int SolvePart1() => -1;
    public sealed override int SolvePart2() => -2;
}}
";

        var mappings = new GeneratedSourceMappings();
        AddProblemMapping(mappings, 2015, 1, baseProblemNamespace);
        AddProblemMapping(mappings, 2015, 2, baseProblemNamespace);
        AddProblemMapping(mappings, 2016, 1, baseProblemNamespace);
        AddProblemMapping(mappings, 2016, 3, baseProblemNamespace);

        AddProblemMapping(mappings, 2015, 3, otherBaseProblemNamespace);
        AddProblemMapping(mappings, 2015, 4, otherBaseProblemNamespace);
        AddProblemMapping(mappings, 2016, 2, otherBaseProblemNamespace);
        AddProblemMapping(mappings, 2016, 4, otherBaseProblemNamespace);

        VerifyAsync(problemClasses, mappings).Wait();
    }

    private const string problemSolutionResourcesNamespace = $"AdventOfCSharp.ProblemSolutionResources.Problems";
    private const string additionalProblemSolutionTestsNamespace = $"AdventOfCSharp.AdditionalProblemSolutionTests.Problems";

    [Test]
    public void SingleReferencedAssembly()
    {
        const string problemClasses =
$@"
using AdventOfCSharp;

namespace {baseProblemNamespace}
{{
    namespace Year2015
    {{
        public sealed class Day1 : TestProblem {{ }}
    }}

    public abstract class TestProblem : Problem<int>
    {{
        public sealed override int SolvePart1() => -1;
        public sealed override int SolvePart2() => -2;
    }}
}}
";

        var mappings = new GeneratedSourceMappings();

        AddProblemMapping(mappings, 2015, 1);

        // From dependency
        AddProblemMapping(mappings, 2021, 1, problemSolutionResourcesNamespace);

        var test = new VerifyCS.Test();
        test.TestState.AdditionalReferences.AddRange(new[]
        {
            // Explicit name for clarity
            MetadataReferenceFactory.CreateFromType<AdventOfCSharp.ProblemSolutionResources.Problems.Year2021.Day1>(),
        });

        VerifyAsync(new[] { problemClasses }, mappings, test).Wait();
    }
    [Test]
    public void TransitivelyReferencedAssemblies()
    {
        const string problemClasses =
$@"
using AdventOfCSharp;

namespace {baseProblemNamespace}
{{
    namespace Year2015
    {{
        public sealed class Day1 : TestProblem {{ }}
    }}

    public abstract class TestProblem : Problem<int>
    {{
        public sealed override int SolvePart1() => -1;
        public sealed override int SolvePart2() => -2;
    }}
}}
";

        var mappings = new GeneratedSourceMappings();

        AddProblemMapping(mappings, 2015, 1);

        // From dependencies
        AddProblemMapping(mappings, 2021, 1, problemSolutionResourcesNamespace);
        AddProblemMapping(mappings, 2021, 2, additionalProblemSolutionTestsNamespace);

        var test = new VerifyCS.Test();
        test.TestState.AdditionalReferences.AddRange(new[]
        {
            // Explicit names for clarity
            MetadataReferenceFactory.CreateFromType<AdventOfCSharp.ProblemSolutionResources.Problems.Year2021.Day1>(),
            MetadataReferenceFactory.CreateFromType<AdventOfCSharp.AdditionalProblemSolutionTests.Problems.Year2021.Day2>(),
        });

        VerifyAsync(new[] { problemClasses }, mappings, test).Wait();
    }

    private static void AddProblemMapping(GeneratedSourceMappings mappings, int year, int day)
    {
        AddProblemMapping(mappings, year, day, baseProblemNamespace);
    }
    private static void AddProblemMapping(GeneratedSourceMappings mappings, int year, int day, string baseNamespace)
    {
        var source = BenchmarkSourceGenerator.GenerateBenchmarkSourceFromBaseNamespace(year, day, baseNamespace);
        var sourceName = BenchmarkSourceGenerator.GetBenchmarkSourceFileName(year, day);

        mappings.Add(sourceName, source);
    }
}
