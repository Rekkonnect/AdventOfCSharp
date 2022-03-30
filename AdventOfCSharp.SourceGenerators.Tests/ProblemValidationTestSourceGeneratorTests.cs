using AdventOfCSharp.SourceGenerators.Tests.Verifiers;
using AdventOfCSharp.SourceGenerators.Utilities;
using AdventOfCSharp.Testing;
using NUnit.Framework;

namespace AdventOfCSharp.SourceGenerators.Tests;

using static ProblemValidationTestSourceGenerator.ConstantNames;
using Test = CSharpSourceGeneratorVerifier<ProblemValidationTestSourceGenerator>.Test;

public sealed class ProblemValidationTestSourceGeneratorTests : BaseSourceGeneratorTestContainer<ProblemValidationTestSourceGenerator>
{
    private const string baseProblemNamespace = "AoC.Test";
    private static readonly string baseTestNamespace = new Test().AssemblyName;

    private const string customBaseDirectory = @"C:\Example\Path";

    [Test]
    public void NUnitTest()
    {
        const string testingAssemblyCode =
$@"
using AdventOfCSharp;
using AdventOfCSharp.Testing;

[assembly: AoCSTestAssembly(@""{customBaseDirectory}"")]

namespace {baseProblemNamespace}.Year2020
{{
    public sealed class Day1 : TestProblem {{ }}
    public sealed class Day2 : TestProblem {{ }}
}}
namespace {baseProblemNamespace}.Year2021
{{
    public sealed class Day1 : TestProblem {{ }}
}}

public abstract class TestProblem : Problem<int>
{{
    public sealed override int SolvePart1() => -1;
    public sealed override int SolvePart2() => -2;
}}
";

        var test = new Test();
        test.AddFrameworkReference(TestingFramework.NUnit);
        
        var mappings = new GeneratedSourceMappings();
        GenerateBaseAssemblyValidationTestsClass(mappings, TestingFramework.NUnit);
        AddGeneratedCaseMappingNUnit(mappings, 2020, 1);
        AddGeneratedCaseMappingNUnit(mappings, 2020, 2);
        AddGeneratedCaseMappingNUnit(mappings, 2021, 1);

        var originalSources = new[]
        {
            testingAssemblyCode
        };
        
        VerifyAsync(originalSources, mappings, test).Wait();
    }

    private static void GenerateBaseAssemblyValidationTestsClass(GeneratedSourceMappings mappings, TestingFramework framework)
    {
        var identifiers = TestingFrameworkIdentifiers.GetForFramework(framework);
        
        var source =
$@"
using {identifiers.AttributeNamespace};
using AdventOfCSharp;
using AdventOfCSharp.Testing.{identifiers.FrameworkNamePrefix};

namespace {baseTestNamespace};

public abstract class {AssemblyProblemValidationTests}<TProblem> : {identifiers.FrameworkNamePrefix}ProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{{
    protected override string GetProblemFileBaseDirectory() => @""{customBaseDirectory}"";
}}
";

        mappings.Add(AssemblyProblemValidationTestsHintName, source);
    }

    private static string GenerateNUnitTestSource(int year, int day)
    {
        return
$@"using NUnit.Framework;

namespace {baseTestNamespace}.Year{year};

public sealed partial class Day{day}ValidationTests : {AssemblyProblemValidationTests}<{baseProblemNamespace}.Year{year}.Day{day}>
{{
    [Test]
    [TestCase(1)]
    [TestCase(2)]
    public void PartTest(int part)
    {{
        PartTestImpl(part);
    }}
}}
";
    }

    private static void AddGeneratedCaseMappingNUnit(GeneratedSourceMappings mappings, int year, int day)
    {
        var source = GenerateNUnitTestSource(year, day);
        AddGeneratedCaseMapping(mappings, year, day, source);
    }
    private static void AddGeneratedCaseMapping(GeneratedSourceMappings mappings, int year, int day, string source)
    {
        var sourceName = ProblemValidationTestSourceGenerator.GetTestCaseSourceFileName(baseTestNamespace, year, day);
        mappings.Add(sourceName, source);
    }
}
