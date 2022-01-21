namespace AdventOfCSharp.Generation;

/// <summary>Provides mechanisms to generate solution class template files.</summary>
public static class SolutionTemplateGeneration
{
    private const string header =
@"namespace [classNamespace];

public sealed class Day[day] : [baseClass]<int>
{
    public override int SolvePart1()
    {
        return -1;
    }";

    private const string footer =
@"

    protected override void LoadState()
    {

    }
    protected override void ResetState()
    {

    }
}
";

    private const string part2Method =
@"
    public override int SolvePart2()
    {
        return -1;
    }";

    private const string solutionClassTemplate = $"{header}{part2Method}{footer}";
    private const string finalDaySolutionClassTemplate = $"{header}{footer}";

    /// <summary>Determines whether the generation of files is enabled. Defaults to <see langword="false"/>.</summary>
    public static bool EnabledGeneration { get; set; } = false;

    /// <summary>Gets or sets the base namespace of the generated solution class. It must be non-<see langword="null"/> when generating files.</summary>
    /// <remarks>The '.YearXXXX' suffix is automatically added during file creation, do not add it on your own.</remarks>
    public static string BaseNamespace { get; set; }
    /// <summary>Gets or sets the base dictionary of the generated solution class. It must be non-<see langword="null"/> when generating files.</summary>
    public static string BaseDirectory { get; set; }

    internal static string InternalBaseNamespace => BaseNamespace ?? throw new InvalidOperationException("The base namespace must be explicitly set.");
    internal static string InternalBaseDirectory => BaseDirectory ?? throw new InvalidOperationException("The base directory must be explicitly set.");

    private static string SolutionClassFileContents(int day, string classNamespace, string? baseClass = null)
    {
        baseClass ??= BaseClassName(day);
        return SolutionClassTemplate(day)
            .Replace($"[{nameof(day)}]", day.ToString())
            .Replace($"[{nameof(baseClass)}]", baseClass)
            .Replace($"[{nameof(classNamespace)}]", classNamespace);
    }
    private static string BaseClassName(int day)
    {
        return day switch
        {
            25 => nameof(FinalDay<int>),
            _ => nameof(Problem<int>),
        };
    }
    private static string SolutionClassTemplate(int day)
    {
        return day switch
        {
            25 => finalDaySolutionClassTemplate,
            _ => solutionClassTemplate,
        };
    }
    private static string GetNamespace(int year, string? baseNamespace = null)
    {
        baseNamespace ??= InternalBaseNamespace;
        return $"{baseNamespace}.Year{year}";
    }
    /// <summary>Generates a solution class file for the specified year, day, base class, base namespace and base dictionary.</summary>
    /// <param name="year">The year of the problem that the solution class aims to solve.</param>
    /// <param name="day">The day of the problem that the solution class aims to solve.</param>
    /// <param name="baseClass">The name of the base class. If <see langword="null"/>, the appropriate framework-provided class name will be used (either <seealso cref="Problem{T}"/> or <seealso cref="FinalDay{T}"/>).</param>
    /// <param name="baseNamespace">The base namespace for the solution class. If <see langword="null"/>, <seealso cref="BaseNamespace"/> will be used.</param>
    /// <param name="baseDirectory">The base directory the file solution class will be contained in. If <see langword="null"/>, <seealso cref="BaseDirectory"/> will be used.</param>
    /// <remarks>
    /// The directory that will contain the resulting file will be the concatenation of the
    /// <seealso cref="BaseDirectory"/> and the <seealso cref="BaseNamespace"/> ('.' will be replaced with directory separators).
    /// Ensure that the directories match the namespaces.
    /// There could also be the need for the base directory to be one level above the project file (depending on the structure).
    /// </remarks>
    public static void CreateSolutionFile(int year, int day, string? baseClass = null, string? baseNamespace = null, string? baseDirectory = null)
    {
        if (!EnabledGeneration)
            return;

        baseDirectory ??= InternalBaseDirectory;
        var classNamespace = GetNamespace(year, baseNamespace);
        var fileName = $"{baseDirectory}/{classNamespace.Replace('.', '/')}/Day{day}.cs";
        var fileContents = SolutionClassFileContents(day, classNamespace, baseClass);
        
        FileHelpers.WriteAllTextEnsuringDirectory(fileName, fileContents);
    }
}
