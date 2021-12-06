#nullable enable

using System.IO;

namespace AdventOfCSharp;

public static class ProblemFiles
{
    private static string? defaultBaseDirectory;

    public static string? CustomBaseDirectory { get; set; }

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
        var entry = Assembly.GetEntryAssembly()!;
        var executableDirectory = Path.GetDirectoryName(entry.Location)!;
        return Directory.GetParent(executableDirectory)!.Parent!.Parent!.ToString();
    }
}
