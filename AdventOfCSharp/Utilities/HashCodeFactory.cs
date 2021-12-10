namespace AdventOfCSharp.Utilities;

public static class HashCodeFactory
{
    public static int Combine<T>(params T[] values) => Combine((IEnumerable<T>)values);
    public static int Combine<T>(IEnumerable<T> values)
    {
        return new HashCode().AddRange(values).ToHashCode();
    }
}
