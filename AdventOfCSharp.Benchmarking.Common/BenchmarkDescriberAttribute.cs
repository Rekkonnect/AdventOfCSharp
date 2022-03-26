using System;
using System.Collections.Immutable;

namespace AdventOfCSharp.Benchmarking;

public sealed class BenchmarkDescriberAttribute : Attribute { }

public abstract class BaseBenchmarkDescriberAttribute : Attribute { }

// Names can be this free because of the context being limited, barely touching other areas
// that could cause confusion to the programmer
// In the context of benchmarking AoC# solutions, the concepts of years, days and dates are
// clear and distinct enough to the consumer
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class YearsAttribute : BaseBenchmarkDescriberAttribute
{
    public ImmutableArray<int> Years { get; }

    public YearsAttribute(params int[] years)
    {
        Years = years.ToImmutableArray();
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DaysAttribute : BaseBenchmarkDescriberAttribute
{
    public ImmutableArray<int> Days { get; }

    public DaysAttribute(params int[] days)
    {
        Days = days.ToImmutableArray();
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DatesAttribute : BaseBenchmarkDescriberAttribute
{
    public int Year { get; }
    public ImmutableArray<int> Days { get; }

    public DatesAttribute(int year, params int[] days)
    {
        Year = year;
        Days = days.ToImmutableArray();
    }
}

// This attribute is specially treated to include all dates,
// completely disregarding other date attributes
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class AllDatesAttribute : BaseBenchmarkDescriberAttribute
{ }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PartsAttribute : BaseBenchmarkDescriberAttribute
{
    public BenchmarkingParts Parts { get; }

    public PartsAttribute(BenchmarkingParts parts)
    {
        Parts = parts;
    }
}
