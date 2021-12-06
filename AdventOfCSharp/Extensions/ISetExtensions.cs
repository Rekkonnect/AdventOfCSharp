namespace AdventOfCSharp.Extensions;

public static class ISetExtensions
{
    public static CollectionOperationResult ToggleElement<T>(this ISet<T> source, T element)
    {
        if (source.Add(element))
            return CollectionOperationResult.Added;

        source.Remove(element);
        return CollectionOperationResult.Removed;
    }
}

public enum CollectionOperationResult
{
    Added,
    Removed,
    // This could be theoretically expanded to support all sorts of operations
}
