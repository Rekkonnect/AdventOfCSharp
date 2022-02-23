using System.Globalization;

namespace AdventOfCSharp.Extensions;

public static class IEnumerableExtensions
{
    public static IDictionary<TKey, TValue> ToDictionaryWithNotNullKeys<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector)
        where TKey : notnull
    {
        return new Dictionary<TKey, TValue>(Pairs());

        IEnumerable<KeyValuePair<TKey, TValue>> Pairs()
        {
            foreach (var value in values)
            {
                var pair = new KeyValuePair<TKey, TValue>(keySelector(value), value);
                if (pair.Key is null)
                    continue;

                yield return pair;
            }
        }
    }
    public static IDictionary<TKey, TValue> ToDictionaryFiltered<TKey, TValue>(this IEnumerable<TValue> values, Func<TValue, TKey> keySelector, Predicate<TValue> valuePredicate)
        where TKey : notnull
    {
        return values.WherePredicate(valuePredicate).ToDictionary(keySelector);
    }

    public static TSource? MinSource<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        where TKey : IComparable<TKey>
    {
        return source.ExtremumSource(selector, ComparisonResult.Less);
    }
    public static TSource? MaxSource<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        where TKey : IComparable<TKey>
    {
        return source.ExtremumSource(selector, ComparisonResult.Greater);
    }

    public static TSource? ExtremumSource<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector, ComparisonResult matchingResult)
        where TKey : IComparable<TKey>
    {
        var first = source.FirstOrDefault();

        if (source.Count() <= 1)
            return first;

        var extremumSelected = selector(first!);
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
