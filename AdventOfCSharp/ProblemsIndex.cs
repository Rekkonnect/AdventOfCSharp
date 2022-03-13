using AdventOfCSharp.Generation;
using AdventOfCSharp.Utilities;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace AdventOfCSharp;

#nullable enable

/// <summary>Provides a handy container for all the available problem solutions.</summary>
public sealed class ProblemsIndex
{
    /// <summary>Gets the singleton instance of the <seealso cref="ProblemsIndex"/>.</summary>
    public static ProblemsIndex Instance { get; } = new();

    private readonly ProblemDictionary problemDictionary = new();

    private ProblemsIndex()
    {
        Console.WriteLine("ProblemsIndex instance created.");

        // This acts like the static constructor since the type is a singleton
        AppDomainHelpers.ForceLoadAllAssembliesCurrent();
        var allClasses = AppDomainCache.Current.AllNonAbstractClasses;

        foreach (var c in allClasses)
            AnalyzeProblemClass(c);

        problemDictionary.DetermineD25P2Availability();

        // Post-instance-initialization analysis
        foreach (var c in allClasses)
        {
            AnalyzeGeneratorClass(c);
        }
    }

    private void AnalyzeProblemClass(Type type)
    {
        var problemType = ProblemType.Parse(type);
        if (problemType is null)
            return;

        Console.WriteLine($"Type found: {type}");
        problemDictionary.SetProblemInfo(GetProblemInfo(problemType));
    }

    private void AnalyzeGeneratorClass(Type type)
    {
        bool isGenerator = type.Inherits(typeof(InputGenerator));
        if (!isGenerator)
            return;

        if (type.InitializeInstance() is not InputGenerator generatorInstance)
            throw new TypeLoadException($"The input generator class '{type.Name}' must provide a public parameterless constructor.");

        this[generatorInstance.Year, generatorInstance.Day].ProblemType.AddGeneratorType(type);
    }

    private static ProblemInfo GetProblemInfo(ProblemType type)
    {
        var part1Status = GetPartSolutionStatus(type, 1);
        var part2Status = GetPartSolutionStatus(type, 2);
        return new ProblemInfo(type, part1Status, part2Status).WithPart2EligibilityFromPart1;
    }
    private static PartSolutionStatus GetPartSolutionStatus(ProblemType type, int index)
    {
        var runnerMethod = ProblemSolverMethodProvider.MethodForPart(type.ProblemClass, index);
        var partSolutionAttribute = runnerMethod.GetCustomAttribute<PartSolutionAttribute>();
        if (partSolutionAttribute is null)
            return PartSolutionStatus.Valid;

        return partSolutionAttribute.Status;
    }

    public ProblemInfo InfoForInstance(Problem instance) => this[instance.Year, instance.Day];

    public IEnumerable<ProblemInfo> AllProblems() => problemDictionary.SelectMany(Selectors.ValueReturner);

    public GlobalYearSummary GetGlobalYearSummary() => problemDictionary.GlobalYearSummary;
    public YearProblemInfo GetYearProblemInfo(int year) => problemDictionary[year];

    public IEnumerable<ProblemDate> GetSingleYearValidDays(int year) => ProblemDate.Dates(year, GetYearProblemInfo(year).ValidDays);
    public IEnumerable<ProblemDate> GetAllValidDates()
    {
        return GetGlobalYearSummary().Select(summary => summary.Year).SelectMany(GetSingleYearValidDays);
    }

    public bool DetermineLastDayPart2Availability(int year) => problemDictionary[year].IsLastDayPart2Available;

    public ProblemInfo this[int year, int day] => problemDictionary[year]?[day] ?? ProblemInfo.Empty(year, day);
    public ProblemInfo this[ProblemDate date] => this[date.Year, date.Day];
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

public sealed class GlobalYearSummary : ProblemInfoBucket<YearSummary, GlobalYearSummary.SummaryTable>
{
    public IEnumerable<int> AvailableYears => this.Select(summary => summary.Year);

    public GlobalYearSummary(IEnumerable<YearSummary> summaries)
        : base(summaries) { }

    protected override int GetBucketIndex(YearSummary info)
    {
        return info.Year;
    }
    protected override YearSummary GetEmptyInfoInstance(int index)
    {
        return YearSummary.Empty(index);
    }

    public sealed class SummaryTable : LookupTable<YearSummary>
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

public sealed class YearProblemInfo : ProblemInfoBucket<ProblemInfo, YearProblemInfo.ProblemInfoTable>
{
    public ProblemInfo LastDay
    {
        get => this[25];
        private set => this[25] = value;
    }

    public IEnumerable<int> ValidDays => this.Select(problemInfo => problemInfo!.Day);

    public bool IsLastDayPart2Available => LastDay.Part2Status is not PartSolutionStatus.UnavailableFreeStar;

    public YearProblemInfo() { }
    public YearProblemInfo(IEnumerable<ProblemInfo> problemInfos)
        : base(problemInfos) { }

    protected override int GetBucketIndex(ProblemInfo info)
    {
        return info.Day;
    }

    public void DetermineD25P2Availability()
    {
        bool anyInvalid = this.Any(problem => !problem.HasBothValidSolutions);
        if (anyInvalid)
            SetD25P2Unavailable();
    }

    private void SetD25P2Unavailable() => LastDay = LastDay.WithUnavailablePart2Star;

    protected override ProblemInfo GetEmptyInfoInstance(int index)
    {
        return ProblemInfo.Empty(0, index);
    }

    public sealed class ProblemInfoTable : LookupTable<ProblemInfo>
    {
        public ProblemInfoTable()
            : base(1, 25) { }
    }
}

public abstract class ProblemInfoBucket<TInfo, TLookupTable> : IEnumerable<TInfo>
    where TInfo : class
    where TLookupTable : LookupTable<TInfo>, new()
{
    private readonly TLookupTable table = new();

    public ProblemInfoBucket() { }
    public ProblemInfoBucket(IEnumerable<TInfo> infos)
    {
        foreach (var info in infos)
            table[GetBucketIndex(info)] = info;
    }

    protected abstract int GetBucketIndex(TInfo info);

    public bool Contains(int index) => table.Contains(index);

    public TInfo this[int index]
    {
        get => table[index] ?? GetEmptyInfoInstance(index);
        set => table[index] = value;
    }

    protected abstract TInfo GetEmptyInfoInstance(int index);

    public IEnumerator<TInfo> GetEnumerator() => table.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed record ProblemType(Type ProblemClass, int Year, int Day)
{
    // TODO: Research regex group name matching system
    public readonly static Regex ProblemClassNameRegex = new(@"Year(?'year'\d*)\.Day(?'day'\d*)$", RegexOptions.Compiled);

    /// <summary>Represents the <seealso cref="InputGenerator"/> type for this problem.</summary>
    public ImmutableArray<Type> GeneratorTypes { get; private set; } = ImmutableArray<Type>.Empty;

    internal void AddGeneratorType(Type generatorType)
    {
        GeneratorTypes = GeneratorTypes.Add(generatorType);
    }

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
    /// <summary>Gets the total count of currently valid solutions, which are marked as <seealso cref="PartSolutionStatus.Valid"/> or <seealso cref="PartSolutionStatus.Unoptimized"/>.</summary>
    public int TotalValidSolutions => this[PartSolutionStatus.Valid] + this[PartSolutionStatus.Unoptimized];
    /// <summary>Gets the total count of solved parts, which are marked as <seealso cref="PartSolutionStatus.Valid"/>, <seealso cref="PartSolutionStatus.Unoptimized"/>, <seealso cref="PartSolutionStatus.Refactoring"/> or <seealso cref="PartSolutionStatus.Interactive"/>.</summary>
    public int TotalSolvedParts => TotalValidSolutions + this[PartSolutionStatus.Refactoring] + this[PartSolutionStatus.Interactive];
    /// <summary>Gets the total count of WIP solutions, which are marked as <seealso cref="PartSolutionStatus.WIP"/> or <seealso cref="PartSolutionStatus.Refactoring"/>.</summary>
    public int TotalWIPSolutions => this[PartSolutionStatus.WIP] + this[PartSolutionStatus.Refactoring];

    public PartSolutionStatusDictionary(IEnumerable<PartSolutionStatus> statuses)
        : base(statuses) { }
}
