namespace AdventOfCSharp.Utilities;

public class NextValueCounterDictionary<T> : ValueCounterDictionary<T>, IEquatable<NextValueCounterDictionary<T>>
{
    public NextValueCounterDictionary() { }
    public NextValueCounterDictionary(IEnumerable<T> collection, int initial = 1)
    {
        foreach (var v in collection)
            Add(v, initial);
    }
    public NextValueCounterDictionary(IEnumerable collection, int initial = 1)
    {
        foreach (var v in collection)
            Add((T)v, initial);
    }
    public NextValueCounterDictionary(NextValueCounterDictionary<T> other) : base(other) { }

    public KeyValuePair<T, int> Max() => Best(ComparisonResult.Greater);
    public KeyValuePair<T, int> Min() => Best(ComparisonResult.Less);

    public KeyValuePair<T, int> Best(ComparisonResult matchingResult)
    {
        KeyValuePair<T, int> best = default;
        int bestValue = int.MaxValue * -(int)matchingResult;

        foreach (var kvp in this)
        {
            var comparisonResult = kvp.Value.GetComparisonResult(bestValue);
            if (comparisonResult == matchingResult)
            {
                best = kvp;
                bestValue = kvp.Value;
            }
            else if (comparisonResult is ComparisonResult.Equal)
            {
                // Reset the best kvp to indicate that there is not a single kvp that has the best value
                best = default;
            }
        }

        return best;
    }

    public int GetFilteredCountersNumber(int value, ComparisonKinds comparison = ComparisonKinds.Equal)
    {
        comparison &= ComparisonKinds.All;

        if (comparison is ComparisonKinds.All)
            return Count;

        if (comparison is ComparisonKinds.None)
            ThrowHelper.Throw<InvalidEnumArgumentException>("There provided comparison type is invalid.");

        return Values.Count(v => v.SatisfiesComparison(value, comparison));
    }

    public bool Equals(NextValueCounterDictionary<T>? other)
    {
        if (other is null)
            return false;

        foreach (var kvp in Dictionary)
            if (!other.Dictionary.TryGetValue(kvp.Key, out int value) || !kvp.Value.Equals(value))
                return false;

        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is NextValueCounterDictionary<T> d && Equals(d);
    }
    public override int GetHashCode()
    {
        // Clearly a "hack" for 
        var result = new HashCode();
        var values = Dictionary.Values.ToArray();
        var sortedValues = values.Sort();
        foreach (var value in sortedValues)
            result.Add(value);
        return result.ToHashCode();
    }
}
