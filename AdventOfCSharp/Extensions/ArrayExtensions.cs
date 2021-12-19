namespace AdventOfCSharp.Extensions;

#nullable disable

public static class ArrayExtensions
{
    /// <summary>Gets the element at the specified indices in the 2D array.</summary>
    /// <typeparam name="T">The type of elements contained in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="index0">The index of dimension 0.</param>
    /// <param name="index1">The index of dimension 1.</param>
    /// <returns>The element at the provided indices in the array.</returns>
    public static T At<T>(this T[,] array, Index index0, Index index1) => array[index0.GetOffset(array, 0), index1.GetOffset(array, 1)];
    /// <summary>Gets the element at the specified indices in the 3D array.</summary>
    /// <typeparam name="T">The type of elements contained in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="index0">The index of dimension 0.</param>
    /// <param name="index1">The index of dimension 1.</param>
    /// <param name="index2">The index of dimension 2.</param>
    /// <returns>The element at the provided indices in the array.</returns>
    public static T At<T>(this T[,,] array, Index index0, Index index1, Index index2) => array[index0.GetOffset(array, 0), index1.GetOffset(array, 1), index2.GetOffset(array, 2)];
    /// <summary>Gets the element at the specified indices in the 4D array.</summary>
    /// <typeparam name="T">The type of elements contained in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="index0">The index of dimension 0.</param>
    /// <param name="index1">The index of dimension 1.</param>
    /// <param name="index2">The index of dimension 2.</param>
    /// <param name="index3">The index of dimension 3.</param>
    /// <returns>The element at the provided indices in the array.</returns>
    public static T At<T>(this T[,,,] array, Index index0, Index index1, Index index2, Index index3) => array[index0.GetOffset(array, 0), index1.GetOffset(array, 1), index2.GetOffset(array, 2), index3.GetOffset(array, 3)];

    /// <summary>Gets the element at the specified indices in the array.</summary>
    /// <param name="array">The array.</param>
    /// <param name="indices">The indices.</param>
    /// <returns>The element at the provided indices in the array.</returns>
    public static object At(this Array array, params Index[] indices) => array.At<object>(indices);
    /// <summary>Gets the element at the specified indices in the array.</summary>
    /// <typeparam name="T">The type of elements contained in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="indices">The indices.</param>
    /// <returns>The element at the provided indices in the array.</returns>
    public static T At<T>(this Array array, params Index[] indices)
    {
        if (indices.Length != array.Rank)
            ThrowHelper.Throw<RankException>("The provided indices do not match the array's rank.");

        var offsets = new int[indices.Length];
        for (int i = 0; i < offsets.Length; i++)
            offsets[i] = indices[i].GetOffset(array, i);
        return (T)array.GetValue(offsets);
    }

    public static TResult[] SelectArray<TSource, TResult>(this TSource[] source, Func<TSource, TResult> selector)
    {
        var result = new TResult[source.Length];

        for (int i = 0; i < result.Length; i++)
            result[i] = selector(source[i]);

        return result;
    }
    public static TResult[,] SelectArray<TSource, TResult>(this TSource[,] source, Func<TSource, TResult> selector)
    {
        var lengths = source.GetDimensionLengths();
        var result = new TResult[lengths[0], lengths[1]];

        for (int i = 0; i < lengths[0]; i++)
            for (int j = 0; j < lengths[1]; j++)
                result[i, j] = selector(source[i, j]);

        return result;
    }

    public static int MaxIndex<T>(this T[] source)
        where T : IComparable<T>
    {
        return source.MaxIndex(out _);
    }
    public static int MinIndex<T>(this T[] source)
        where T : IComparable<T>
    {
        return source.MinIndex(out _);
    }
    public static int ExtremumIndex<T>(this T[] source, ComparisonResult targetResult)
        where T : IComparable<T>
    {
        return source.ExtremumIndex(targetResult, out _);
    }

    public static int MaxIndex<T>(this T[] source, out T max)
        where T : IComparable<T>
    {
        return source.ExtremumIndex(ComparisonResult.Greater, out max);
    }
    public static int MinIndex<T>(this T[] source, out T min)
        where T : IComparable<T>
    {
        return source.ExtremumIndex(ComparisonResult.Less, out min);
    }

    public static int ExtremumIndex<T>(this T[] source, ComparisonResult targetResult, out T extremum)
        where T : IComparable<T>
    {
        int index = 0;
        extremum = source[0];
        for (int i = 1; i < source.Length; i++)
        {
            if (source[i].MatchesComparisonResult(extremum, targetResult))
            {
                extremum = source[i];
                index = i;
            }
        }

        return index;
    }

    public static T[] RotateRight<T>(this T[] source, int rotation)
    {
        int length = source.Length;
        rotation %= length;

        var result = new T[length];

        for (int i = 0; i < length; i++)
            result[(i + rotation) % length] = source[i];

        return result;
    }
    public static T[] RotateLeft<T>(this T[] source, int rotation)
    {
        int length = source.Length;
        rotation %= length;
        return source.RotateRight(length - rotation);
    }

    public static T[] Replace<T>(this T[] source, T[] replacement, int start, int length)
    {
        int end = start + length;
        var replaced = new T[source.Length - length + replacement.Length];

        Array.Copy(source, replaced, start);
        Array.Copy(replacement, 0, replaced, start, replacement.Length);
        Array.Copy(source, end, replaced, start + replacement.Length, source.Length - end);

        return replaced;
    }
    public static T[] Replace<T>(this T[] source, T replacement, int start, int length)
    {
        int end = start + length;
        var replaced = new T[source.Length - length + 1];

        Array.Copy(source, replaced, start);
        replaced[start] = replacement;
        Array.Copy(source, end, replaced, start + 1, source.Length - end);

        return replaced;
    }
}
