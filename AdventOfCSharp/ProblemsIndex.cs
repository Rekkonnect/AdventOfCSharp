using AdventOfCSharp.Utilities;
using System.Text.RegularExpressions;

namespace AdventOfCSharp;

#nullable enable

public class ProblemsIndex
{
    public static ProblemsIndex Instance { get; } = new();

    private readonly ProblemDictionary problemDictionary = new();

    private ProblemsIndex()
    {
        var allClasses = AppDomainCache.Current.AllNonAbstractClasses;
        foreach (var c in allClasses)
            AnalyzeProblemClass(c);
        problemDictionary.DetermineD25P2Availability();
    }

    private void AnalyzeProblemClass(Type type)
    {
        var problemType = ProblemType.Parse(type);
        if (problemType is null)
            return;

        problemDictionary.SetProblemInfo(GetProblemInfo(problemType));
    }

    private static ProblemInfo GetProblemInfo(ProblemType type)
    {
        var part1Status = GetPartSolutionStatus(type, 1);
        var part2Status = GetPartSolutionStatus(type, 2);
        return new ProblemInfo(type, part1Status, part2Status).WithPart2EligibilityFromPart1;
    }
    private static PartSolutionStatus GetPartSolutionStatus(ProblemType type, int index)
    {
        var runnerMethod = type.ProblemClass.GetMethod($"{ProblemRunner.SolvePartMethodPrefix}{index}")!;
        var partSolutionAttribute = runnerMethod.GetCustomAttribute<PartSolutionAttribute>();
        if (partSolutionAttribute is null)
            return PartSolutionStatus.Valid;

        return partSolutionAttribute.Status;
    }

    public IEnumerable<ProblemInfo> AllProblems() => problemDictionary.SelectMany(Selectors.ValueReturner);

    public GlobalYearSummary GetGlobalYearSummary() => problemDictionary.GlobalYearSummary;
    public YearProblemInfo GetYearProblemInfo(int year) => problemDictionary[year];

    public bool DetermineLastDayPart2Availability(int year) => problemDictionary[year].IsLastDayPart2Available;

    public ProblemInfo this[int year, int day] => problemDictionary[year]?[day] ?? ProblemInfo.Empty(year, day);
}

// Consider creating another custom lookup table wrapper type
// Because that's what I've been dreaming programming of
// Creating custom lookup table wrapper types
public sealed class ProblemDictionary : FlexibleInitializableValueDictionary<int, YearProblemInfo>
{
    public GlobalYearSummary GlobalYearSummary => new(this.Select(kvp => new YearSummary(kvp.Key, kvp.Value)));

    public void SetProblemInfo(ProblemInfo info)
    {
        this[info.ProblemType.Year][info.ProblemType.Day] = info;
    }

    public void DetermineD25P2Availability()
    {
        foreach (var yearProblemInfo in Values)
            yearProblemInfo.DetermineD25P2Availability();
    }
}

// This and YearProblemInfo can be abstracted away
public sealed class GlobalYearSummary : IEnumerable<YearSummary>
{
    private readonly SummaryTable summaryTable = new();

    public IEnumerable<int> AvailableYears => summaryTable.Select(summary => summary!.Year);

    public GlobalYearSummary(IEnumerable<YearSummary> summaries)
    {
        foreach (var summary in summaries)
            summaryTable[summary.Year] = summary;
    }

    public bool Contains(int year) => summaryTable.Contains(year);

    public YearSummary this[int year] => summaryTable[year] ?? YearSummary.Empty(year);

    public IEnumerator<YearSummary> GetEnumerator() => summaryTable.GetEnumerator() as IEnumerator<YearSummary>;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class SummaryTable : LookupTable<YearSummary?>
    {
        private static readonly int finalYear = DateTime.Now.Year;

        public SummaryTable()
            : base(2015, finalYear) { }
    }
}

public sealed class YearSummary
{
    public int Year { get; }
    public PartSolutionStatusDictionary StatusCounters { get; }

    public bool HasAvailableSolutions => StatusCounters.TotalValidSolutions > 0;

    public YearSummary(int year, IEnumerable<ProblemInfo> problemInfos)
    {
        Year = year;
        StatusCounters = new(problemInfos.SelectMany(info => info.PartStatuses));
    }

    public static YearSummary Empty(int year) => new(year, Enumerable.Empty<ProblemInfo>());
}

public sealed class YearProblemInfo : IEnumerable<ProblemInfo>
{
    private readonly ProblemInfoTable summaryTable = new();

    public ProblemInfo LastDay
    {
        get => this[25];
        private set => this[25] = value;
    }

    public bool IsLastDayPart2Available => LastDay.Part2Status is not PartSolutionStatus.UnavailableFreeStar;

    public YearProblemInfo() { }
    public YearProblemInfo(IEnumerable<ProblemInfo> problemInfos)
    {
        foreach (var problemInfo in problemInfos)
            summaryTable[problemInfo.Day] = problemInfo;
    }

    public bool Contains(int day) => summaryTable.Contains(day);

    public void DetermineD25P2Availability()
    {
        bool anyInvalid = this.Reverse().Any(problem => !problem.HasBothValidSolutions);
        if (anyInvalid)
            SetD25P2Unavailable();
    }

    private void SetD25P2Unavailable() => LastDay = LastDay.WithUnavailablePart2Star;

    public ProblemInfo this[int day]
    {
        get => summaryTable[day] ?? ProblemInfo.Empty(0, day);
        set => summaryTable[day] = value;
    }

    public IEnumerator<ProblemInfo> GetEnumerator() => summaryTable.GetEnumerator() as IEnumerator<ProblemInfo>;
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private sealed class ProblemInfoTable : LookupTable<ProblemInfo?>
    {
        public ProblemInfoTable()
            : base(1, 25) { }
    }
}

public sealed record ProblemType(Type ProblemClass, int Year, int Day)
{
    // TODO: Research regex group name matching system
    public readonly static Regex ProblemClassNameRegex = new(@"Year(?'year'\d*)\.Day(?'day'\d*)$", RegexOptions.Compiled);

    public Problem? InitializeInstance() => ProblemClass?.InitializeInstance<Problem>();

    public static ProblemType Mock(int year, int day) => new(null!, year, day);

    public static ProblemType? Parse(Type problemClass)
    {
        var match = ProblemClassNameRegex.Match(problemClass.FullName!);
        if (!match.Success)
            return null;

        int year = match.Groups["year"].Value.ParseInt32();
        int day = match.Groups["day"].Value.ParseInt32();

        return new(problemClass, year, day);
    }
}

public sealed record ProblemInfo(ProblemType ProblemType, PartSolutionStatus Part1Status, PartSolutionStatus Part2Status)
{
    private PartSolutionStatus[]? lazyStatuses;
    public IEnumerable<PartSolutionStatus> PartStatuses => lazyStatuses ??= new[] { Part1Status, Part2Status };

    public int Year => ProblemType.Year;
    public int Day => ProblemType.Day;

    // Usually it's P2 that isn't solved, a tiny tiny tiny optimization for the fuck of it
    public bool HasBothValidSolutions => Part2Status.IsValidSolution() && Part1Status.IsValidSolution();
    public bool HasNoValidSolutions => !Part2Status.IsValidSolution() && !Part1Status.IsValidSolution();

    public ProblemInfo WithPart2EligibilityFromPart1 => Part1Status.HasBeenSolved() ? this : WithUninitializedPart2;

    public ProblemInfo WithUnavailablePart2Star => this with { Part2Status = PartSolutionStatus.UnavailableFreeStar };
    public ProblemInfo WithUninitializedPart2 => this with { Part2Status = PartSolutionStatus.Uninitialized };

    public Problem? InitializeInstance() => ProblemType.InitializeInstance();

    public PartSolutionStatus StatusForPart(int part) => part switch
    {
        1 => Part1Status,
        2 => Part2Status,
    };

    public static ProblemInfo Empty(int year, int day) => new(ProblemType.Mock(year, day), PartSolutionStatus.Uninitialized, PartSolutionStatus.Uninitialized);
}

// Optimizable, but for no reason
public sealed class PartSolutionStatusDictionary : ValueCounterDictionary<PartSolutionStatus>
{
    public int TotalValidSolutions => this[PartSolutionStatus.Valid] + this[PartSolutionStatus.Unoptimized];
    public int TotalSolvedParts => TotalValidSolutions + this[PartSolutionStatus.Refactoring];
    public int TotalWIPSolutions => this[PartSolutionStatus.WIP] + this[PartSolutionStatus.Refactoring];

    public PartSolutionStatusDictionary(IEnumerable<PartSolutionStatus> statuses)
        : base(statuses) { }
}
