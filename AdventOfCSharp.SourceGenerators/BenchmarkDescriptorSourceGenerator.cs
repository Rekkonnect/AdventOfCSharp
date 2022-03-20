using AdventOfCSharp.Benchmarking;
using AdventOfCSharp.CodeAnalysis.Core;
using AdventOfCSharp.SourceGenerators.Extensions;
using AdventOfCSharp.SourceGenerators.Utilities;
using Microsoft.CodeAnalysis;
using RoseLynn;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace AdventOfCSharp.SourceGenerators;

// TODO: Consider implementing this generator as an incremental one
[Generator]
public class BenchmarkDescriptorSourceGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var defined = context.Compilation.GetAllDefinedTypes();
        var benchmarkDescribers = defined.Where(IsBenchmarkDescriber);

        foreach (var describer in benchmarkDescribers)
        {
            var source = GenerateBenchmarkDescriberSource(context, describer);
            context.AddSource(GetBenchmarkSourceFileName(describer), source);
        }
    }

    private static bool IsBenchmarkDescriber(INamedTypeSymbol classSymbol)
    {
        return AoCSAnalysisHelpers.IsImportantAoCSClass(classSymbol, KnownSymbolNames.BenchmarkDescriber);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public static string GetBenchmarkSourceFileName(string describerNamespace, string describerName)
    {
        return GetBenchmarkSourceFileName($"{describerNamespace}.{describerName}");
    }
    public static string GetBenchmarkSourceFileName(string fullSymbolName)
    {
        return $"{fullSymbolName}.Implementation.g.cs";
    }
    public static string GetBenchmarkSourceFileName(INamedTypeSymbol benchmarkType)
    {
        return GetBenchmarkSourceFileName(benchmarkType.GetFullSymbolName().FullNameString);
    }

    private static string GenerateBenchmarkDescriberSource(GeneratorExecutionContext context, INamedTypeSymbol benchmarkType)
    {
        var generator = new BenchmarkDescriberImplementationGenerator(context.Compilation, benchmarkType);
        return generator.GenerateBenchmarkDescriberSource();
    }

    private sealed class DeclaredDatesRuleSystem
    {
        private readonly bool allDatesEnabled;

        private readonly ImmutableArray<int> years;
        private readonly ImmutableArray<int> days;
        private readonly ImmutableArray<ProblemDate> specificDates;

        public DeclaredDatesRuleSystem(IEnumerable<int>? years, IEnumerable<int>? days, IEnumerable<ProblemDate>? dates)
        {
            this.years = years?.ToImmutableArray() ?? default;
            this.days = days?.ToImmutableArray() ?? default;
            specificDates = dates?.ToImmutableArray() ?? default;
        }
        public DeclaredDatesRuleSystem(bool allDates)
        {
            allDatesEnabled = allDates;
        }

        public static DeclaredDatesRuleSystem WithAllDatesEnabled() => new(true);

        public bool IsValidDate(ProblemDate date)
        {
            if (allDatesEnabled)
                return true;

            return IsValidDateByFilters(date);
        }
        public IEnumerable<ProblemDate> FilterDates(IEnumerable<ProblemDate> allDates)
        {
            if (allDatesEnabled)
                return allDates;

            // Slightly annoying that this is about O(n logn)
            return allDates.Where(date => IsValidDateByFilters(date));
        }

        private bool IsValidDateByFilters(ProblemDate date)
        {
            return years.Contains(date.Year)
                || days.Contains(date.Day)
                || specificDates.Contains(date);
        }
    }

    private sealed class BenchmarkDescriberImplementationGenerator
    {
        private readonly StringBuilder sourceBuilder = new();

        private readonly Compilation compilation;
        private readonly INamedTypeSymbol benchmarkType;
        private readonly ProblemClassDeclarationCorrelationCollection correlations;
        private readonly DeclaredDatesRuleSystem declaredDates;

        private ImmutableArray<ProblemDate> benchmarkedDates;
        private BenchmarkingParts benchmarkedParts = BenchmarkingParts.OnlyParts;

        public BenchmarkDescriberImplementationGenerator(Compilation compilation, INamedTypeSymbol benchmarkType)
        {
            this.compilation = compilation;
            this.benchmarkType = benchmarkType;

            correlations = ProblemClassIdentifier.GetProblemClassCorrelations(compilation);

            AnalyzeDates();
            AnalyzeBenchmarkedParts();
            declaredDates = GetDeclaredDates();
        }

        private DeclaredDatesRuleSystem GetDeclaredDates()
        {
            bool allDatesEnabled = benchmarkType.HasAttributeNamed<AllDatesAttribute>();
            if (allDatesEnabled)
            {
                return DeclaredDatesRuleSystem.WithAllDatesEnabled();
            }

            int[] years = null;
            int[] days = null;
            List<ProblemDate> specificDates = null;

            var yearsAttribute = benchmarkType.FirstOrDefaultAttributeNamed<YearsAttribute>();
            if (yearsAttribute is not null)
            {
                years = yearsAttribute.ConstructorArguments[0].Values.Select(value => (int)value.Value).ToArray();
            }

            var daysAttribute = benchmarkType.FirstOrDefaultAttributeNamed<DaysAttribute>();
            if (daysAttribute is not null)
            {
                days = daysAttribute.ConstructorArguments[0].Values.Select(value => (int)value.Value).ToArray();
            }

            var datesAttributes = benchmarkType.GetAttributesNamed<DatesAttribute>();
            foreach (var dateAttribute in datesAttributes)
            {
                int year = (int)dateAttribute.ConstructorArguments[0].Value;
                var matchedDays = dateAttribute.ConstructorArguments[1].Values.Select(value => (int)value.Value).ToArray();
                var matchedDates = ProblemDate.Dates(year, matchedDays);
                specificDates.AddRange(matchedDates);
            }

            return new(years, days, specificDates);
        }
        private void AnalyzeDates()
        {
            benchmarkedDates = declaredDates.FilterDates(correlations.Dates).ToImmutableArray();
        }
        private void AnalyzeBenchmarkedParts()
        {
            var partsAttribute = benchmarkType.FirstOrDefaultAttributeNamed<PartsAttribute>();
            if (partsAttribute is null)
                return;

            benchmarkedParts = (BenchmarkingParts)partsAttribute.ConstructorArguments[0].Value;
        }

        public string GenerateBenchmarkDescriberSource()
        {
            var describerNamespace = benchmarkType.GetFullSymbolName().FullNamespaceString;

            var header =
$@"
using AdventOfCSharp.Benchmarking;
using BenchmarkDotNet.Attributes;

#nullable disable

namespace {describerNamespace}
{{
    partial class {benchmarkType.Name}
    {{
";
            var footer =
$@"
    }}
}}
";

            sourceBuilder.Append(header);

            GenerateAllBenchmarkFields();
            GenerateSetup();
            GenerateBenchmarkMethods();

            return sourceBuilder.Append(footer).ToString();
        }

        private string GetFieldPrefix(ProblemDate date)
        {
            return $"year{date.Year}day{date.Day}";
        }

        private void GenerateAllBenchmarkFields()
        {
            foreach (var date in benchmarkedDates)
                GenerateBenchmarkFields(date);
        }
        private void GenerateBenchmarkFields(ProblemDate date)
        {
            var (year, day) = date;
            string fieldPrefix = GetFieldPrefix(date);
            var correlation = ProblemClassIdentifier.GetProblemClassCorrelations(compilation)[year, day];

            // Not too clean, but not too slow either
            sourceBuilder.AppendLine();
            sourceBuilder
                .Append(@"        private readonly Problem ").Append(fieldPrefix).Append(" = new ").Append(correlation.FullSymbolName).AppendLine("();")
                .Append(@"        private Action ").Append(fieldPrefix).Append("part1, ")
                                                   .Append(fieldPrefix).Append("part2, ")
                                                   .Append(fieldPrefix).AppendLine("input;");
        }

        private void GenerateBenchmarkMethods()
        {
            foreach (var date in benchmarkedDates)
                ConditionallyGenerateBenchmarkMethods(date);
        }
        private void ConditionallyGenerateBenchmarkMethods(ProblemDate date)
        {
            ConditionallyGenerateBenchmarkMethod(date, BenchmarkingParts.Input);
            ConditionallyGenerateBenchmarkMethod(date, BenchmarkingParts.Part1);
            ConditionallyGenerateBenchmarkMethod(date, BenchmarkingParts.Part2);
        }
        private void ConditionallyGenerateBenchmarkMethod(ProblemDate date, BenchmarkingParts part)
        {
            if ((part & benchmarkedParts) is 0)
                return;

            GenerateBenchmarkMethod(date, part);
        }
        private void GenerateBenchmarkMethod(ProblemDate date, BenchmarkingParts part)
        {
            GenerateBenchmarkMethod(date, GetPartName(part));
        }
        private static string GetPartName(BenchmarkingParts part) => part switch
        {
            BenchmarkingParts.Input => nameof(BenchmarkingParts.Input),
            BenchmarkingParts.Part1 => nameof(BenchmarkingParts.Part1),
            BenchmarkingParts.Part2 => nameof(BenchmarkingParts.Part2),
        };

        private void GenerateBenchmarkMethod(ProblemDate date, string part)
        {
            var (year, day) = date;
            string lowercasePart = part.ToLower();
            string doubleDigitDay = day.ToString("N2");

            sourceBuilder.AppendLine();
            sourceBuilder
                .Append(@"        [Benchmark]")
                .Append(@"        [BenchmarkCategory(""Year ").Append(year).Append(" Day ").Append(doubleDigitDay).AppendLine("\")]")
                .Append(@"        public void Year").Append(year).Append("_Day").Append(doubleDigitDay).Append("_").Append(part).AppendLine("()")
                .AppendLine("        {")
                .Append(@"           year").Append(year).Append("day").Append(doubleDigitDay).Append(lowercasePart).AppendLine("();")
                .AppendLine("        }");
        }

        private const string setupActionsName = "SetupActions";

        private void GenerateSetup()
        {
            sourceBuilder.AppendLine(
$@"
        [GlobalSetup]
        public void Setup()
        {{
            {setupActionsName}();
        }}
");

            GenerateSetupActions();
        }
        private void GenerateSetupActions()
        {
            sourceBuilder.AppendLine();
            sourceBuilder
                .AppendLine($@"        private void {setupActionsName}()")
                .AppendLine(@"        {");

            foreach (var date in benchmarkedDates)
                GenerateSetupAction(date);

            sourceBuilder
                .AppendLine(@"        }");
        }
        private void GenerateSetupAction(ProblemDate date)
        {
            var fieldPrefix = GetFieldPrefix(date);

            sourceBuilder
                .Append(@"        CreateAssignBenchmarkedActions(").Append(fieldPrefix)
                .Append(@", ref ").Append(fieldPrefix).Append("part1")
                .Append(@", ref ").Append(fieldPrefix).Append("part2")
                .Append(@", ref ").Append(fieldPrefix).Append("input")
                .AppendLine(");");
        }
    }
}
