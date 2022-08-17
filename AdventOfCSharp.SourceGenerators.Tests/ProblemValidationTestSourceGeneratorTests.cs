using AdventOfCSharp.SourceGenerators.Tests.Verifiers;
using AdventOfCSharp.SourceGenerators.Utilities;
using AdventOfCSharp.Testing;
using NUnit.Framework;
using RoseLynn.Generators;
using System.ComponentModel;
using TestIdentity;

namespace AdventOfCSharp.SourceGenerators.Tests;

using static ProblemValidationTestSourceGenerator.ConstantNames;
using Test = CSharpSourceGeneratorVerifier<ProblemValidationTestSourceGenerator>.Test;

public sealed class ProblemValidationTestSourceGeneratorTests : BaseSourceGeneratorTestContainer<ProblemValidationTestSourceGenerator>
{
    private const string baseProblemNamespace = "AoC.Test";
    private static readonly string baseTestNamespace = new Test().AssemblyName;

    private const string customBaseDirectory = @"C:\Example\Path";

    private const string testingAssemblyCode =
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

    [Test]
    [TestCase(TestingFramework.NUnit)]
    [TestCase(TestingFramework.XUnit)]
    [TestCase(TestingFramework.MSTest)]
    public void GeneralTestingFrameworks(TestingFramework testingFramework)
    {
        var test = new Test();
        test.AddFrameworkReference(testingFramework);
        
        var mappings = new GeneratedSourceMappings();
        GenerateBaseAssemblyValidationTestsClass(mappings, testingFramework);
        AddGeneratedCaseMapping(mappings, testingFramework, 2020, 1);
        AddGeneratedCaseMapping(mappings, testingFramework, 2020, 2);
        AddGeneratedCaseMapping(mappings, testingFramework, 2021, 1);

        VerifyAsync(testingAssemblyCode, mappings, test).Wait();
    }

    private static void GenerateBaseAssemblyValidationTestsClass(GeneratedSourceMappings mappings, TestingFramework framework)
    {
        var identifiers = TestingFrameworkIdentifiers.GetForFramework(framework);
        
        var source =
$@"
using {identifiers.AttributeNamespace};
using AdventOfCSharp;
using AdventOfCSharp.Testing.{identifiers.CodeFrameworkName};

namespace {baseTestNamespace};

public abstract class {AssemblyProblemValidationTests}<TProblem> : {identifiers.CodeFrameworkName}ProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{{
    protected override string GetProblemFileBaseDirectory() => @""{customBaseDirectory}"";
}}
";

        mappings.Add(AssemblyProblemValidationTestsHintName, source);
    }

    private static string GenerateTestSource(TestingFramework framework, int year, int day)
    {
        return framework switch
        {
            TestingFramework.NUnit => GenerateNUnitTestSource(year, day),
            TestingFramework.XUnit => GenerateXUnitTestSource(year, day),
            TestingFramework.MSTest => GenerateMSTestTestSource(year, day),

            _ => throw new InvalidEnumArgumentException("An unknown testing framework was requested."),
        };
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
    private static string GenerateXUnitTestSource(int year, int day)
    {
        return
$@"using Xunit;

namespace {baseTestNamespace}.Year{year};

public sealed partial class Day{day}ValidationTests : {AssemblyProblemValidationTests}<{baseProblemNamespace}.Year{year}.Day{day}>
{{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void PartTest(int part)
    {{
        PartTestImpl(part);
    }}
}}
";
    }
    private static string GenerateMSTestTestSource(int year, int day)
    {
        return
$@"using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace {baseTestNamespace}.Year{year};

[TestClass]
public sealed partial class Day{day}ValidationTests : {AssemblyProblemValidationTests}<{baseProblemNamespace}.Year{year}.Day{day}>
{{
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void PartTest(int part)
    {{
        PartTestImpl(part);
    }}
}}
";
    }

    private static void AddGeneratedCaseMapping(GeneratedSourceMappings mappings, TestingFramework framework, int year, int day)
    {
        var source = GenerateTestSource(framework, year, day);
        AddGeneratedCaseMapping(mappings, year, day, source);
    }
    private static void AddGeneratedCaseMapping(GeneratedSourceMappings mappings, int year, int day, string source)
    {
        var sourceName = ProblemValidationTestSourceGenerator.GetTestCaseSourceFileName(baseTestNamespace, year, day);
        mappings.Add(sourceName, source);
    }
}
