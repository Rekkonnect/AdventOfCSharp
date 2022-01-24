namespace AdventOfCSharp;

/// <summary>Provides mechanisms for running problem solutions.</summary>
public sealed class ProblemRunner
{
    public static readonly string SolvePartMethodPrefix = nameof(Problem<int>.SolvePart1)[..^1];

    /// <summary>The problem instance that is being run.</summary>
    public Problem Problem { get; }

    public ProblemRunner(Problem problem)
    {
        Problem = problem;
    }

    private static ProblemRunner? ForInstance(Problem? instance)
    {
        if (instance is null)
            return null;

        return new(instance);
    }

    /// <summary>Creates a new <seealso cref="ProblemRunner"/> instance for the problem of the specified day.</summary>
    /// <param name="year">The year of the problem.</param>
    /// <param name="day">The day of the problem.</param>
    /// <returns>A <seealso cref="ProblemRunner"/> instance for the specified problem, if a solution class is available for it, otherwise <see langword="null"/>.</returns>
    public static ProblemRunner? ForProblem(int year, int day) => ForInstance(ProblemsIndex.Instance[year, day].InitializeInstance());

    // Too many displayExecutionTimes parameters; could be handled from some property
    public PartSolutionOutputDictionary SolveAllParts(bool displayExecutionTimes = true) => SolveAllParts(0, displayExecutionTimes);
    public PartSolutionOutputDictionary SolveAllParts(int testCase, bool displayExecutionTimes = true)
    {
        var methods = Problem.GetType().GetMethods().Where(m => m.HasCustomAttribute<PartSolverAttribute>()).ToArray();
        return SolveParts(testCase, methods, displayExecutionTimes);
    }

    public object SolvePart(int part, bool displayExecutionTimes = true) => SolvePart(part, 0, displayExecutionTimes);
    public object SolvePart(int part, int testCase, bool displayExecutionTimes = true)
    {
        var methods = new[] { Problem.GetType().GetMethod(SolvePartMethodName(part))! };
        return SolveParts(testCase, methods, displayExecutionTimes).GetPartOutput(part)!;
    }

    public bool FullyValidateAllTestCases(bool displayExecutionTimes = true)
    {
        foreach (int testCase in Problem.Input.TestCaseIDs)
            if (!ValidateAllParts(testCase, displayExecutionTimes))
                return false;

        return true;
    }
    public bool ValidateAllParts(bool displayExecutionTimes = true)
    {
        return ValidateAllParts(0, displayExecutionTimes);
    }
    public bool ValidateAllParts(int testCase, bool displayExecutionTimes = true)
    {
        return ValidatePart(1, testCase, displayExecutionTimes) && ValidatePart(2, testCase, displayExecutionTimes);
    }

    public bool ValidatePart(int part, bool displayExecutionTimes = true) => ValidatePart(part, 0, displayExecutionTimes);
    public bool ValidatePart(int part, int testCase, bool displayExecutionTimes = true)
    {
        var contents = Problem.Input.GetOutputFileContents(testCase, true);
        var expectedPartOutput = contents.ForPart(part);
        if (expectedPartOutput is null)
            return true;

        return ValidatePart(part, testCase, expectedPartOutput, displayExecutionTimes);
    }
    private bool ValidatePart(int part, int testCase, string expected, bool displayExecutionTimes)
    {
        return expected.Equals(AnswerStringConversion.Convert(SolvePart(part, testCase, displayExecutionTimes)), StringComparison.OrdinalIgnoreCase);
    }

    private static string SolvePartMethodName(int part) => ExecutePartMethodName(SolvePartMethodPrefix, part);
    private static string ExecutePartMethodName(string prefix, int part) => $"{prefix}{part}";

    private PartSolutionOutputDictionary SolveParts(int testCase, MethodInfo[] solutionMethods, bool displayExecutionTimes)
    {
        var result = new PartSolutionOutputDictionary();

        Problem.CurrentTestCase = testCase;

        var stateLoader = Problem.GetType().GetMethod("LoadState", BindingFlags.NonPublic | BindingFlags.Instance)!;
        bool inputPrints = MethodPrints(stateLoader);
        RunDisplayExecutionTimes(displayExecutionTimes, inputPrints, "Input", FancyPrinting.PrintCustomPartLabel, Problem.EnsureLoadedState);

        for (int i = 0; i < solutionMethods.Length; i++)
        {
            var method = solutionMethods[i];
            bool prints = MethodPrints(method);
            var partName = method.GetCustomAttribute<PartSolverAttribute>()!.PartName;

            RunDisplayExecutionTimes(displayExecutionTimes, prints, partName, FancyPrinting.GetPartLabelPrinter(partName), SolveAssignResult);

            void SolveAssignResult()
            {
                var output = solutionMethods[i].Invoke(Problem, null)!;
                result.Add(partName, output);
            }
        }
        return result;
    }

    private static bool MethodPrints(MethodInfo method)
    {
        return method.HasCustomAttribute<PrintsToConsoleAttribute>()
            || method.GetCustomAttribute<PartSolutionAttribute>() is { Status: PartSolutionStatus.Interactive };
    }

    private static void RunDisplayExecutionTimes(bool displayExecutionTimes, bool prints, string partName, FancyPrinting.PartLabelPrinter printer, Action runner)
    {
        bool defaultLivePrintingSetting = ExecutionTimePrinting.EnableLivePrinting;
        if (prints)
            ExecutionTimePrinting.EnableLivePrinting = false;

        if (displayExecutionTimes)
        {
            printer(partName);
            ExecutionTimePrinting.BeginExecutionMeasuring();
        }

        runner();

        if (displayExecutionTimes)
        {
            ExecutionTimePrinting.StopExecutionMeasuring().Wait();
        }

        ExecutionTimePrinting.EnableLivePrinting = defaultLivePrintingSetting;
    }
}

public sealed class PartSolutionOutputDictionary : IEnumerable<PartSolutionOutputEntry>
{
    private readonly Dictionary<string, object> dictionary = new();

    public object? Part1Output { get; private set; }
    public object? Part2Output { get; private set; }

    public PartSolutionOutputDictionary() { }
    public PartSolutionOutputDictionary(IEnumerable<PartSolutionOutputEntry> entries)
    {
        AddRange(entries);
    }

    public void AddRange(IEnumerable<PartSolutionOutputEntry> entries)
    {
        foreach (var entry in entries)
            Add(entry);
    }
    public void Add(PartSolutionOutputEntry entry)
    {
        Add(entry.PartName, entry.Output);
    }
    public void Add(string partName, object output)
    {
        dictionary.Add(partName, output);
        AssignOfficialPartResult(partName, output);
    }
    private void AssignOfficialPartResult(string partName, object output)
    {
        switch (partName)
        {
            case "Part 1":
                Part1Output = output;
                break;

            case "Part 2":
                Part2Output = output;
                break;
        }
    }

    public object? GetPartOutput(int part)
    {
        return part switch
        {
            1 => Part1Output,
            2 => Part2Output,
            _ => null,
        };
    }

    public IEnumerator<PartSolutionOutputEntry> GetEnumerator()
    {
        return dictionary.Select(PartSolutionOutputEntry.FromKeyValuePair).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public record struct PartSolutionOutputEntry(string PartName, object Output)
{
    public static PartSolutionOutputEntry FromKeyValuePair(KeyValuePair<string, object> kvp) => new(kvp.Key, kvp.Value);
}
