namespace AdventOfCSharp.Extensions;

public static class IEnumerableExtensions
{
    public static bool CountAtLeast<T>(this IEnumerable<T> source, Func<T, bool> filter, int occurrences)
    {
        var filtered = source.Where(filter);
        int count = 0;

        foreach (var e in filtered)
        {
            count++;
            if (count >= occurrences)
                return true;
        }
        return false;
    }

    public static TSource MinSource<TSource, TSelector>(this IEnumerable<TSource> source, Func<TSource, TSelector> selector)
        where TSelector : IComparable<TSelector>
    {
        return source.ExtremumSource(selector, ComparisonResult.Less);
    }
    public static TSource MaxSource<TSource, TSelector>(this IEnumerable<TSource> source, Func<TSource, TSelector> selector)
        where TSelector : IComparable<TSelector>
    {
        return source.ExtremumSource(selector, ComparisonResult.Greater);
    }

    public static TSource ExtremumSource<TSource, TSelector>(this IEnumerable<TSource> source, Func<TSource, TSelector> selector, ComparisonResult matchingResult)
        where TSelector : IComparable<TSelector>
    {
        var first = source.FirstOrDefault();

        if (source.Count() <= 1)
            return first;

        var extremumSelected = selector(first);
        var extremumSource = first;

        foreach (var sourceValue in source.Skip(1))
        {
            var selected = selector(sourceValue);
            var comparison = selected.GetComparisonResult(extremumSelected);

            if (comparison == matchingResult)
            {
                extremumSource = sourceValue;
                extremumSelected = selected;
            }
            else if (comparison is ComparisonResult.Equal)
            {
                extremumSource = default;
            }
        }

        return extremumSource;
    }
}
