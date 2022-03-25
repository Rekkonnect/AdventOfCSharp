#nullable enable

using System.IO;

namespace AdventOfCSharp;

public static class ResourceFileHelpers
{
    public static string GetBaseCodeDirectory()
    {
        var entry = Assembly.GetEntryAssembly()!;
        var executableDirectory = Path.GetDirectoryName(entry.Location)!;
        return Directory.GetParent(executableDirectory)!.Parent!.Parent!.ToString();
    }
}
