using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCSharp.Utilities;

#nullable enable

public class LookupTable<T> : IReadOnlyCollection<T>
{
    protected readonly int Offset;
    protected readonly T?[] Values;

    public int Start => Offset;
    public int End => Offset + Count;

    public int Count => Values.Length;

    public IEnumerable<T> NonNullValues => Values.Where(NotNull) as IEnumerable<T>;

    public LookupTable(int start, int end)
    {
        Offset = start;
        Values = new T[end - start + 1];
    }
    public LookupTable(LookupTable<T> other)
        : this(other.Start, other.End)
    {
        other.Values.CopyTo(Values, 0);
    }

    private static bool NotNull<U>(U value) => value is not null;

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

    public IEnumerator<T> GetEnumerator() => NonNullValues.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
