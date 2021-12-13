namespace AdventOfCSharp.Extensions;

public static class DictionaryEntryExtensions
{
    public static KeyValuePair<TKey, TValue> ToKeyValuePair<TKey, TValue>(this DictionaryEntry entry)
    {
        return new((TKey)entry.Key, (TValue)entry.Value!);
    }
}
