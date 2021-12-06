#nullable enable

namespace AdventOfCSharp.Extensions;

public static class StringExtensions
{
    /// <summary>Evaluates whether the given string is not <see langword="null"/> or empty.</summary>
    /// <param name="s">The string to evaluate.</param>
    /// <returns><see langword="null"/> if the given string is <see langword="null"/> or empty, otherwise the string itself.</returns>
    public static string? NullIfEmpty(this string s)
    {
        return s.IsNullOrEmpty() ? null : s;
    }
    /// <summary>Evaluates whether the given string is not <see langword="null"/> or empty.</summary>
    /// <param name="s">The string to evaluate.</param>
    /// <returns><see langword="true"/> if the given string is <see langword="null"/> or empty, otherwise <see langword="false"/>.</returns>
    public static bool IsNullOrEmpty(this string s)
    {
        return string.IsNullOrEmpty(s);
    }
}
