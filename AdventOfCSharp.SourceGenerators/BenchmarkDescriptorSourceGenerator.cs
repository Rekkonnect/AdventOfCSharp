using AdventOfCSharp.Benchmarking;
using AdventOfCSharp.CodeAnalysis.Core;
using AdventOfCSharp.SourceGenerators.Extensions;
using AdventOfCSharp.SourceGenerators.Utilities;
using Microsoft.CodeAnalysis;
using RoseLynn;
using RoseLynn.Utilities;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace AdventOfCSharp.SourceGenerators;

[Generator]
public class BenchmarkDescriberSourceGenerator : ISourceGenerator
{
    private readonly Dictionary<Compilation, CompilationBenchmarkDescribers> generatorExecutions = new();

    public void Execute(GeneratorExecutionContext context)
    {
        // Overwrite previously registered, if existing
        var describers = new CompilationBenchmarkDescribers(context.Compilation);
        generatorExecutions[context.Compilation] = describers;

        var defined = context.Compilation.GetAllDefinedTypes();
        var benchmarkDescribers = defined.Where(IsBenchmarkDescriber);

        foreach (var describer in benchmarkDescribers)
        {
            var source = GenerateBenchmarkDescriberSource(describer);
            context.AddSource(GetBenchmarkSourceFileName(describer), source);
        }

        string GenerateBenchmarkDescriberSource(INamedTypeSymbol benchmarkType)
        {
            var generator = new BenchmarkDescriberImplementationGenerator(context.Compilation, benchmarkType);
            generatorExecutions[context.Compilation].Add(generator.Info);
            return generator.GenerateBenchmarkDescriberSource();
        }

        static bool IsBenchmarkDescriber(INamedTypeSymbol classSymbol)
        {
            return AoCSAnalysisHelpers.IsImportantAoCSClass(classSymbol, KnownFullSymbolNames.BenchmarkDescriber);
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public CompilationBenchmarkDescribers GetExecutionResults(Compilation compilation)
    {
        return generatorExecutions[compilation];
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

    private sealed class DeclaredDatesRuleSystem
    {
        private readonly bool allDatesEnabled;

        private readonly ImmutableArray<int> years;
        private readonly ImmutableArray<int> days;
        private readonly ImmutableArray<ProblemDate> specificDates;

        public DeclaredDatesRuleSystem(IEnumerable<int>? years, IEnumerable<int>? days, IEnumerable<ProblemDate>? dates)
        {
            this.years = years?.ToImmutableArray() ?? ImmutableArray<int>.Empty;
            this.days = days?.ToImmutableArray() ?? ImmutableArray<int>.Empty;
            specificDates = dates?.ToImmutableArray() ?? ImmutableArray<ProblemDate>.Empty;
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

    public sealed class CompilationBenchmarkDescribers
    {
        private readonly Dictionary<INamedTypeSymbol, BenchmarkDescriberInfo> describers = new(SymbolEqualityComparer.Default);

        public Compilation Compilation { get; }

        public IEnumerable<BenchmarkDescriberInfo> Info => describers.Values;

        public CompilationBenchmarkDescribers(Compilation compilation)
        {
            Compilation = compilation;
        }

        public void Add(BenchmarkDescriberInfo describer)
        {
            describers.Add(describer.DescriberSymbol, describer);
        }

        public BenchmarkDescriberInfo this[INamedTypeSymbol type] => describers.ValueOrDefault(type);
    }

    public sealed class BenchmarkDescriberInfo
    {
        public INamedTypeSymbol DescriberSymbol { get; }

        public ImmutableArray<ProblemDate> Dates { get; }
        public BenchmarkingParts BenchmarkingParts { get; }

        public BenchmarkDescriberInfo(INamedTypeSymbol describerSymbol, ImmutableArray<ProblemDate> dates, BenchmarkingParts parts)
        {
            DescriberSymbol = describerSymbol;
            Dates = dates;
            BenchmarkingParts = parts;
        }
        public BenchmarkDescriberInfo(INamedTypeSymbol describerSymbol, IEnumerable<ProblemDate> dates, BenchmarkingParts parts)
            : this(describerSymbol, dates.ToImmutableArray(), parts) { }
    }

    private sealed class BenchmarkDescriberImplementationGenerator
    {
        private readonly StringBuilder sourceBuilder = new();

        private readonly Compilation compilation;
        private readonly INamedTypeSymbol describerType;
        private readonly ProblemClassDeclarationCorrelationCollection correlations;
        private readonly DeclaredDatesRuleSystem declaredDates;

        public BenchmarkDescriberInfo Info { get; private set; }

        public BenchmarkDescriberImplementationGenerator(Compilation compilation, INamedTypeSymbol benchmarkType)
        {
            this.compilation = compilation;
            describerType = benchmarkType;

            correlations = ProblemClassIdentifier.GetProblemClassCorrelations(compilation);

            declaredDates = GetDeclaredDates();
            AnalyzeInfo();
        }

        private DeclaredDatesRuleSystem GetDeclaredDates()
        {
            bool allDatesEnabled = describerType.HasAttributeNamed<AllDatesAttribute>();
            if (allDatesEnabled)
            {
                return DeclaredDatesRuleSystem.WithAllDatesEnabled();
            }

            int[] years = null;
            int[] days = null;
            var specificDates = new List<ProblemDate>();

            var yearsAttribute = describerType.FirstOrDefaultAttributeNamed<YearsAttribute>();
            if (yearsAttribute is not null)
            {
                years = yearsAttribute.ConstructorArguments[0].Values.Select(value => (int)value.Value).ToArray();
            }

            var daysAttribute = describerType.FirstOrDefaultAttributeNamed<DaysAttribute>();
            if (daysAttribute is not null)
            {
                days = daysAttribute.ConstructorArguments[0].Values.Select(value => (int)value.Value).ToArray();
            }

            var datesAttributes = describerType.GetAttributesNamed<DatesAttribute>();
            foreach (var dateAttribute in datesAttributes)
            {
                int year = (int)dateAttribute.ConstructorArguments[0].Value;
                var matchedDays = dateAttribute.ConstructorArguments[1].Values.Select(value => (int)value.Value).ToArray();
                var matchedDates = ProblemDate.Dates(year, matchedDays);
                specificDates.AddRange(matchedDates);
            }

            return new(years, days, specificDates);
        }
        private void AnalyzeInfo()
        {
            var benchmarkedDates = declaredDates.FilterDates(correlations.Dates).ToImmutableArray();
            var benchmarkedParts = BenchmarkingParts.OnlyParts;

            var partsAttribute = describerType.FirstOrDefaultAttributeNamed<PartsAttribute>();
            if (partsAttribute is not null)
            {
                benchmarkedParts = (BenchmarkingParts)partsAttribute.ConstructorArguments[0].Value;
            }

            Info = new(describerType, benchmarkedDates, benchmarkedParts);
        }

        public string GenerateBenchmarkDescriberSource()
        {
            var describerNamespace = describerType.GetFullSymbolName().FullNamespaceString;

            var header =
$@"
using AdventOfCSharp;
using AdventOfCSharp.Benchmarking;
using BenchmarkDotNet.Attributes;
using System;

#nullable disable

namespace {describerNamespace}
{{
    partial class {describerType.Name}
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
            foreach (var date in Info.Dates)
                GenerateBenchmarkFields(date);
        }
        private void GenerateBenchmarkFields(ProblemDate date)
        {
            var (year, day) = date;
            string fieldPrefix = GetFieldPrefix(date);
            var correlation = ProblemClassIdentifier.GetProblemClassCorrelations(compilation)[year, day];

            // Not too clean, but not too slow either
            sourceBuilder
                .Append(@"        private readonly Problem ").Append(fieldPrefix).Append(" = new ").Append(correlation.FullSymbolName).AppendLine("();")
                .Append(@"        private Action ").Append(fieldPrefix).Append("part1, ")
                                                   .Append(fieldPrefix).Append("part2, ")
                                                   .Append(fieldPrefix).AppendLine("input;");
        }

        private void GenerateBenchmarkMethods()
        {
            foreach (var date in Info.Dates)
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
            if ((part & Info.BenchmarkingParts) is 0)
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
            string doubleDigitDay = day.ToString("D2");

            sourceBuilder.AppendLine();
            sourceBuilder
                .AppendLine(@"        [Benchmark]")
                .Append(@"        [BenchmarkCategory(""Year ").Append(year).Append(" Day ").Append(doubleDigitDay).AppendLine("\")]")
                .Append(@"        public void Year").Append(year).Append("_Day").Append(doubleDigitDay).Append("_").Append(part).AppendLine("()")
                .AppendLine("        {")
                .Append(@"            year").Append(year).Append("day").Append(day).Append(lowercasePart).AppendLine("();")
                .Append("        }");
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
            sourceBuilder
                .AppendLine($@"        private void {setupActionsName}()")
                .AppendLine(@"        {");

            foreach (var date in Info.Dates)
                GenerateSetupAction(date);

            sourceBuilder
                .AppendLine(@"        }");
        }
        private void GenerateSetupAction(ProblemDate date)
        {
            var fieldPrefix = GetFieldPrefix(date);

            sourceBuilder
                .Append(@"            CreateAssignBenchmarkedActions(").Append(fieldPrefix)
                .Append(@", ref ").Append(fieldPrefix).Append("part1")
                .Append(@", ref ").Append(fieldPrefix).Append("part2")
                .Append(@", ref ").Append(fieldPrefix).Append("input")
                .AppendLine(");");
        }
    }
}
