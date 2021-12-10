namespace AdventOfCSharp.Utilities;

public class LookupTable<T> : IReadOnlyCollection<T>
{
    protected readonly int Offset;
    protected readonly T?[] Values;

    public int Count => Values.Length;

    public IEnumerable<T> NonNullValues => Values.Where(Predicates.NotNull);

    public LookupTable(int start, int end)
    {
        Offset = start;
        Values = new T[end - start + 1];
    }

    public bool Contains(int index)
    {
        return ValidIndex(index) && this[index] is not null;
    }

    public T? ValueOrDefault(int index)
    {
        if (ValidIndex(index))
            return this[index];

        return default;
    }
    public bool SetIfValidIndex(int index, T? value)
    {
        bool valid = ValidIndex(index);
        if (valid)
            this[index] = value;
        return valid;
    }

    public bool ValidIndex(int index)
    {
        var arrayIndex = index - Offset;
        return arrayIndex >= 0 && arrayIndex < Values.Length;
    }

    public virtual T? this[int index]
    {
        get => Values[index - Offset];
        set => Values[index - Offset] = value;
    }

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)Values).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
