using System.IO;
using System.Threading.Tasks;

namespace AdventOfCSharp;

/// <summary>Provides some helper functions to operate on files.</summary>
public static class FileHelpers
{
    /// <summary>Writes all lines to a file at the specified path, while ensuring that its parent directory exists, by creating it if it doesn't.</summary>
    /// <param name="path">The path of the file that will be created.</param>
    /// <param name="lines">The lines to write to the file.</param>
    public static void WriteAllLinesEnsuringDirectory(string path, IEnumerable<string> lines)
    {
        EnsureDirectoryForPath(path);
        File.WriteAllLines(path, lines);
    }
    /// <summary>Writes all text to a file at the specified path, while ensuring that its parent directory exists, by creating it if it doesn't.</summary>
    /// <param name="path">The path of the file that will be created.</param>
    /// <param name="contents">The contents to write to the file.</param>
    public static void WriteAllTextEnsuringDirectory(string path, string contents)
    {
        EnsureDirectoryForPath(path);
        File.WriteAllText(path, contents);
    }

    /// <summary>Asynchronously writes all lines to a file at the specified path, while ensuring that its parent directory exists, by creating it if it doesn't.</summary>
    /// <param name="path">The path of the file that will be created.</param>
    /// <param name="lines">The lines to write to the file.</param>
    /// <returns>A <seealso cref="Task"/> that represents the operation of creating ensuring the parent directory's existence, and the <see langword="async"/> write operation.</returns>
    public static async Task WriteAllLinesEnsuringDirectoryAsync(string path, IEnumerable<string> lines)
    {
        EnsureDirectoryForPath(path);
        await File.WriteAllLinesAsync(path, lines);
    }
    /// <summary>Asynchronously writes all text to a file at the specified path, while ensuring that its parent directory exists, by creating it if it doesn't.</summary>
    /// <param name="path">The path of the file that will be created.</param>
    /// <param name="contents">The contents to write to the file.</param>
    /// <returns>A <seealso cref="Task"/> that represents the operation of creating ensuring the parent directory's existence, and the <see langword="async"/> write operation.</returns>
    public static async Task WriteAllTextEnsuringDirectoryAsync(string path, string contents)
    {
        EnsureDirectoryForPath(path);
        await File.WriteAllTextAsync(path, contents);
    }

    private static void EnsureDirectoryForPath(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
    }
}
