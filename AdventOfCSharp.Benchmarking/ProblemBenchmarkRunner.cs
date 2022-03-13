using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Parameters;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Garyon.Extensions;
using System.Reflection;

namespace AdventOfCSharp.Benchmarking;

public static class ProblemBenchmarkRunner
{
    public static IEnumerable<ProblemDate> Dates { get; set; }

    public static void AppendDates(IEnumerable<ProblemDate> dates) => Dates = Dates.Concat(dates);

    public static void SetFlattenedDates(IEnumerable<IEnumerable<ProblemDate>> collections)
    {
        Dates = collections.Flatten();
    }

    public static void SetSingleYear(int year)
    {
        Dates = ProblemsIndex.Instance.GetSingleYearValidDays(year);
    }
    public static void IncludeAllProblems()
    {
        Dates = ProblemsIndex.Instance.GetAllValidDates();
    }

    public static void RunAllProblems()
    {
        IncludeAllProblems();
        RunSelectedProblems();
    }

    private static readonly ParameterDefinition yearParameterDefinition = new(nameof(ProblemBenchmark.Year), false, null, false, typeof(int), 1);
    private static readonly ParameterDefinition dayParameterDefinition = new(nameof(ProblemBenchmark.Day), false, null, false, typeof(int), 2);
    private static readonly ImmutableConfig minimumImmutableConfig = ImmutableConfigBuilder.Create(ManualConfig.CreateMinimumViable());
    private static readonly Type benchmarkType = typeof(ProblemBenchmark);
    private static readonly MethodInfo setupMethodInfo = typeof(ProblemBenchmark).GetMethod(nameof(ProblemBenchmark.Setup))!;

    private static void RunSelectedProblems()
    {
        BenchmarkRunner.Run(GenerateRunInfo(Dates));
    }
    public static void DebugRunSelectedProblems()
    {
        BenchmarkRunner.Run(GenerateRunInfo(Dates, new DebugInProcessConfig()));
    }

    private static BenchmarkRunInfo GenerateRunInfo(IEnumerable<ProblemDate> dates, IConfig? config = null)
    {
        var immutableConfig = minimumImmutableConfig;
        if (config is not null)
            immutableConfig = ImmutableConfigBuilder.Create(config);
        
        return new BenchmarkRunInfo(GenerateCases(dates).ToArray(), typeof(ProblemBenchmark), immutableConfig);
    }

    private static IEnumerable<BenchmarkCase> GenerateCases(IEnumerable<ProblemDate> dates) => dates.SelectMany(GenerateCases);

    private static IEnumerable<BenchmarkCase> GenerateCases(ProblemDate date)
    {
        return GenerateCases(GetParameterInstances(date));
    }
    private static ParameterInstances GetParameterInstances(ProblemDate date)
    {
        return new(new[]
        {
            new ParameterInstance(yearParameterDefinition, date.Year, SummaryStyle.Default),
            new ParameterInstance(dayParameterDefinition, date.Day, SummaryStyle.Default),
        });
    }

    private static IEnumerable<BenchmarkCase> GenerateCases(ParameterInstances instances)
    {
        return new[]
        {
            GenerateCase(nameof(ProblemBenchmark.Input), instances),
            GenerateCase(nameof(ProblemBenchmark.Part1), instances),
            GenerateCase(nameof(ProblemBenchmark.Part2), instances),
        };
    }

    private static BenchmarkCase GenerateCase(string methodName, ParameterInstances problemDateInstances)
    {
        return BenchmarkCase.Create(GetProblemBenchmarkDescriptor(methodName), Job.Default, problemDateInstances, minimumImmutableConfig);
    }
    private static Descriptor GetProblemBenchmarkDescriptor(string methodName)
    {
        return new Descriptor(
            benchmarkType,
            benchmarkType.GetMethod(methodName),
            globalSetupMethod: setupMethodInfo);
    }
}
