namespace AdventOfCSharp;

/// <summary>Contains information about the problem files, including inputs and outputs.</summary>
public static class ProblemFiles
{
    private const string baseDirectoryEnvironmentVariableName = "AoCS_ProblemFiles_BaseDirectory";

    private static string? defaultBaseDirectory;

    public static string DefaultBaseDirectory
    {
        get
        {
            if (defaultBaseDirectory is null)
                return defaultBaseDirectory = CalculateDefaultBaseDirectory();

            return defaultBaseDirectory;
        }
    }

    /// <summary>Gets or sets the custom base directory. A <see langword="null"/> value will resort to using the default base directory.</summary>
    public static string? CustomBaseDirectory { get; set; }

    /// <summary>Gets the base directory of the problem files.</summary>
    /// <returns>If <seealso cref="CustomBaseDirectory"/> is not <see langword="null"/>, it will override the default base directory. Otherwise, the default base directory will be used.</returns>
    public static string GetBaseDirectory()
    {
        var environmentVariable = ReadBaseDirectoryFromEnvironmentVariable();

        if (CustomBaseDirectory is not null)
            return CustomBaseDirectory;

        if (environmentVariable is not null)
            return environmentVariable;

        return DefaultBaseDirectory;
    }

    public static string? ReadBaseDirectoryFromEnvironmentVariable()
    {
        var environmentVariable = Environment.GetEnvironmentVariable(baseDirectoryEnvironmentVariableName);
        CustomBaseDirectory ??= environmentVariable;
        return environmentVariable;
    }
    public static void DumpBaseDirectoryToEnvironmentVariable()
    {
        if (CustomBaseDirectory is null)
            return;

        Environment.SetEnvironmentVariable(baseDirectoryEnvironmentVariableName, CustomBaseDirectory);
    }

    public static void SetCustomBaseDirectorySyncEnvironmentVariable(string? customBaseDirectory)
    {
        CustomBaseDirectory = customBaseDirectory;
        DumpBaseDirectoryToEnvironmentVariable();
    }
    public static void RestoreBaseDirectoryClearEnvironmentVariable(string? previousCustomBaseDirectory)
    {
        CustomBaseDirectory = previousCustomBaseDirectory;
        Environment.SetEnvironmentVariable(baseDirectoryEnvironmentVariableName, null);
    }

    private static string CalculateDefaultBaseDirectory()
    {
        return ResourceFileHelpers.GetBaseCodeDirectory();
    }
}
