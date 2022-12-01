using AdventOfCSharp.SourceGenerators.Utilities;
using AdventOfCSharp.Testing;
using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCSharp.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class ProblemValidationTestSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var iterator = new TestClassGenerator(context);
        iterator.GenerateAllTests();
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public static string GetTestCaseSourceFileName(string baseTestNamespace, int year, int day)
    {
        return GetTestCaseSourceFileName($"{baseTestNamespace}.Year{year}.Day{day}{ConstantNames.ValidationTestsSuffix}");
    }
    public static string GetTestCaseSourceFileName(string fullSymbolName)
    {
        return $"{fullSymbolName}.g.cs";
    }

    private sealed class TestClassGenerator
    {
        private const string testingFrameworkNameGroupName = "framework";
        private const string AoCSTestingPrefix = $"{nameof(AdventOfCSharp)}.{nameof(AdventOfCSharp.Testing)}.";

        private static readonly Regex testingDependencyNamePattern = new($@"{AoCSTestingPrefix}(?'{testingFrameworkNameGroupName}'\w*)$");

        private readonly TestingFrameworkIdentifiers identifiers;
        private readonly ProblemClassDeclarationCorrelationCollection correlations;
        private readonly string baseTestNamespace;
        private string problemFileBaseDirectory;

        private readonly GeneratorExecutionConductor executionConductor;

        public GeneratorExecutionContext Context { get; }

        public bool Usable { get; } = true;

        public TestClassGenerator(GeneratorExecutionContext context)
        {
            Context = context;
            executionConductor = new(context);

            Usable = ScanProblemFileBaseDirectory();
            if (!Usable)
                return;

            baseTestNamespace = context.Compilation.AssemblyName!;
            identifiers = ScanTestingFrameworkIdentifiers();
            Usable = identifiers is not null;
            if (!Usable)
                return;

            correlations = ProblemClassIdentifier.GetProblemClassCorrelations(Context.Compilation);
        }

#nullable enable

        private bool ScanProblemFileBaseDirectory()
        {
            var testAssemblyAttribute = GetAssemblyTestAttribute();
            if (testAssemblyAttribute is null)
                return false;

            problemFileBaseDirectory = testAssemblyAttribute.ConstructorArguments[0].Value as string;
            return true;
        }
        private AttributeData? GetAssemblyTestAttribute()
        {
            return Context.Compilation.Assembly.FirstOrDefaultAttributeNamed<AoCSTestAssemblyAttribute>();
        }

        private TestingFrameworkIdentifiers? ScanTestingFrameworkIdentifiers()
        {
            var AoCSTestingDependencyNames = Context.Compilation.ReferencedAssemblyNames
                .Select(identity => identity.Name)
                .Where(name => name.StartsWith(AoCSTestingPrefix));

            var matches = AoCSTestingDependencyNames
                .Select(name => testingDependencyNamePattern.Match(name))
                .Where(match => match.Success)
                .ToArray();

            // The user has not added any test framework; abort
            if (matches.Length is 0)
                return null!;

            // As long as the user uses at least one of the frameworks, they are meant to work
            var frameworkNames = matches.Select(match => match.Groups[testingFrameworkNameGroupName].Value);
            var identifier = frameworkNames
                .Select(TestingFrameworkIdentifiers.GetForFrameworkName)
                .FirstOrDefault(identifier => identifier is not null);

            return identifier;
        }

        public void GenerateAllTests()
        {
            if (!Usable)
                return;

            GenerateBaseAssemblyTestClass();

            foreach (var correlation in correlations.Correlations)
            {
                GenerateTestClass(correlation);
            }

            executionConductor.FinalizeClearGeneratorExecution();
        }

        private void GenerateBaseAssemblyTestClass()
        {
            var baseDirectoryMethodOverride = $@"    protected override string GetProblemFileBaseDirectory() => @""{problemFileBaseDirectory}"";";

            string header =
$@"
using {identifiers.AttributeNamespace};
using AdventOfCSharp;
using AdventOfCSharp.Testing.{identifiers.FrameworkNamePrefix};

namespace {baseTestNamespace};

public abstract class {ConstantNames.AssemblyProblemValidationTests}<TProblem> : {identifiers.FrameworkNamePrefix}ProblemValidationTests<TProblem>
    where TProblem : Problem, new()
{{
";
            string footer =
$@"
}}
";

            var code = header;
            if (problemFileBaseDirectory is not null)
                code += baseDirectoryMethodOverride;
            code += footer;

            executionConductor.AddSource(ConstantNames.AssemblyProblemValidationTestsHintName, code);
        }

        private void GenerateTestClass(ProblemClassDeclarationCorrelation correlation)
        {
            bool hasValid = correlation.HasQualityAssertableParts(out bool part1, out bool part2);
            if (!hasValid)
                return;

            var testNamespace = $"{baseTestNamespace}.Year{correlation.Year}";
            var className = $"Day{correlation.Day}{ConstantNames.ValidationTestsSuffix}";

            // Header
            var builder = new StringBuilder();
            builder.AppendLine($"using {identifiers.AttributeNamespace};").AppendLine()
                   .AppendLine($"namespace {testNamespace};").AppendLine();

            // Class attribute
            if (identifiers.TestClassAttributeName is not null)
            {
                builder.AppendLine($"[{identifiers.TestClassAttributeName}]");
            }

            // Class declaration
            builder.Append("public sealed partial class ").Append(className)
                   .Append(" : ").Append(ConstantNames.AssemblyProblemValidationTests).Append('<').Append(correlation.FullSymbolName).AppendLine(">");

            builder.AppendLine("{");

            // Method attributes
            builder.Append("    [").Append(identifiers.ParameterizedTestMethodAttributeName).AppendLine("]");
            AppendPartAttribute(part1, 1);
            AppendPartAttribute(part2, 2);

            // Method declaration + footer
            builder.AppendLine(
@"    public void PartTest(int part)
    {
        PartTestImpl(part);
    }
}");

            var hintName = GetTestCaseSourceFileName($"{testNamespace}.{className}");
            executionConductor.AddSource(hintName, builder.ToString());

            void AppendPartAttribute(bool validPart, int partNumber)
            {
                if (!validPart)
                    return;

                builder.Append("    [").Append(identifiers.InlineDataAttributeName).Append("(").Append(partNumber).AppendLine(")]");
            }
        }
    }

    public static class ConstantNames
    {
        public const string AssemblyProblemValidationTests = nameof(AssemblyProblemValidationTests);
        public const string AssemblyProblemValidationTestsHintName = $"{AssemblyProblemValidationTests}.g.cs";

        public const string ValidationTestsSuffix = "ValidationTests";
    }
}
