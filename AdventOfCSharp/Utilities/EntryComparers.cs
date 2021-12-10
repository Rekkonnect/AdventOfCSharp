namespace AdventOfCSharp.Utilities;

// Straight to Garyon
public interface IKeyValuePairComparer<TKey, TValue> : IComparer<KeyValuePair<TKey, TValue>> { }

public interface IFullKeyValuePairComparer<TKey, TValue> : IKeyValuePairComparer<TKey, TValue>
    where TKey : IComparable<TKey>
    where TValue : IComparable<TValue>
{
}

public sealed class EntryKeyOverValueComparer<TKey, TValue> : IFullKeyValuePairComparer<TKey, TValue>
    where TKey : IComparable<TKey>
    where TValue : IComparable<TValue>
{
    public static readonly EntryKeyOverValueComparer<TKey, TValue> Default = new();

    public int Compare(KeyValuePair<TKey, TValue> left, KeyValuePair<TKey, TValue> right)
    {
        int comparison = left.Key.CompareTo(right.Key);
        if (comparison != 0)
            return comparison;

        return left.Value.CompareTo(right.Value);
    }
}

public sealed class EntryValueOverKeyComparer<TKey, TValue> : IFullKeyValuePairComparer<TKey, TValue>
    where TKey : IComparable<TKey>
    where TValue : IComparable<TValue>
{
    public static readonly EntryValueOverKeyComparer<TKey, TValue> Default = new();

    public int Compare(KeyValuePair<TKey, TValue> left, KeyValuePair<TKey, TValue> right)
    {
        int comparison = left.Value.CompareTo(right.Value);
        if (comparison != 0)
            return comparison;

        return left.Key.CompareTo(right.Key);
    }
}

public sealed class EntryKeyComparer<TKey, TValue> : IKeyValuePairComparer<TKey, TValue>
    where TKey : IComparable<TKey>
{
    public static readonly EntryKeyComparer<TKey, TValue> Default = new();

    public int Compare(KeyValuePair<TKey, TValue> left, KeyValuePair<TKey, TValue> right)
    {
        return left.Key.CompareTo(right.Key);
    }
}

public sealed class EntryValueComparer<TKey, TValue> : IKeyValuePairComparer<TKey, TValue>
    where TValue : IComparable<TValue>
{
    public static readonly EntryValueComparer<TKey, TValue> Default = new();

    public int Compare(KeyValuePair<TKey, TValue> left, KeyValuePair<TKey, TValue> right)
    {
        return left.Value.CompareTo(right.Value);
    }
}
