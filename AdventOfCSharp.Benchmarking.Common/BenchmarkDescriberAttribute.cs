using System;
using System.Collections.Immutable;

namespace AdventOfCSharp.Benchmarking;

public abstract class BenchmarkDescriberAttribute : Attribute { }

// Names can be this free because of the context being limited, barely touching other areas
// that could cause confusion to the programmer
// In the context of benchmarking AoC# solutions, the concepts of years, days and dates are
// clear and distinct enough to the consumer
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class YearsAttribute : BenchmarkDescriberAttribute
{
    public ImmutableArray<int> Years { get; }

    public YearsAttribute(params int[] years)
    {
        Years = years.ToImmutableArray();
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DaysAttribute : BenchmarkDescriberAttribute
{
    public ImmutableArray<int> Days { get; }

    public DaysAttribute(params int[] days)
    {
        Days = days.ToImmutableArray();
    }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DatesAttribute : BenchmarkDescriberAttribute
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
public sealed class AllDatesAttribute : BenchmarkDescriberAttribute
{ }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PartsAttribute : BenchmarkDescriberAttribute
{
    public BenchmarkingParts Parts { get; }

    public PartsAttribute(BenchmarkingParts parts)
    {
        Parts = parts;
    }
}
