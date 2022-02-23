using System.Resources;

namespace AdventOfCSharp.Extensions;

public static class ResourceSetExtensions
{
    public static IEnumerable<DictionaryEntry> AsEntries(this ResourceSet set)
    {
        return set.Cast<DictionaryEntry>();
    }

    public static IEnumerable<KeyValuePair<string, string>> GetStringEntries(this ResourceSet set)
    {
        return set.AsEntries().Where(entry => entry.Value is string).Select(DictionaryEntryExtensions.ToKeyValuePair<string, string>);
    }
    public static IEnumerable<string> GetStrings(this ResourceSet set)
    {
        return set.AsEntries().Select(entry => entry.Value).OfType<string>();
    }
}
