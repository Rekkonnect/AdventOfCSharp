namespace AdventOfCSharp;

/// <summary>Contains information about the problem files, including inputs and outputs.</summary>
public static class ProblemFiles
{
    private static string? defaultBaseDirectory;

    /// <summary>Gets or sets the custom base directory. A <see langword="null"/> value will resort to using the default base directory.</summary>
    public static string? CustomBaseDirectory { get; set; }

    /// <summary>Gets the base directory of the problem files.</summary>
    /// <returns>If <seealso cref="CustomBaseDirectory"/> is not <see langword="null"/>, it will override the default base directory. Otherwise, the default base directory will be used.</returns>
    public static string GetBaseDirectory()
    {
        if (CustomBaseDirectory is not null)
            return CustomBaseDirectory;

        if (defaultBaseDirectory is not null)
            return defaultBaseDirectory;

        return defaultBaseDirectory = GetDefaultBaseDirectory();
    }

    private static string GetDefaultBaseDirectory()
    {
        return ResourceFileHelpers.GetBaseCodeDirectory();
    }
}
