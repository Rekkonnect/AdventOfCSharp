namespace AdventOfCSharp.Extensions;

public static class IDictionaryEnumeratorExtensions
{
    public static IEnumerable<T> Values<T>(this IDictionary enumerator)
    {
        return enumerator.Values.Cast<T>();
    }
}
