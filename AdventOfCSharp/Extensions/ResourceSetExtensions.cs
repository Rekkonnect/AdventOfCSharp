using System.Resources;

namespace AdventOfCSharp.Extensions;

public static class ResourceSetExtensions
{
    public static IEnumerable<string> GetStrings(this ResourceSet set)
    {
        return set.Cast<DictionaryEntry>().Select(entry => entry.Value).OfType<string>();
    }
}
