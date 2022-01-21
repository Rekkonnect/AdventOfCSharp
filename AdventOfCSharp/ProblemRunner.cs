namespace AdventOfCSharp;

public sealed class ProblemRunner
{
    public static readonly string RunPartMethodPrefix = nameof(Problem<int>.RunPart1)[..^1];
    public static readonly string SolvePartMethodPrefix = nameof(Problem<int>.SolvePart1)[..^1];

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

    public static ProblemRunner? ForProblem(int year, int day) => ForInstance(ProblemsIndex.Instance[year, day].InitializeInstance());

    // Too many displayExecutionTimes parameters; could be handled from some property
    public object[] SolveAllParts(bool displayExecutionTimes = true) => SolveAllParts(0, displayExecutionTimes);
    public object[] SolveAllParts(int testCase, bool displayExecutionTimes = true)
    {
        var methods = Problem.GetType().GetMethods().Where(m => m.Name.StartsWith(SolvePartMethodPrefix)).ToArray();
        return SolveParts(testCase, methods, displayExecutionTimes);
    }

    public object SolvePart(int part, bool displayExecutionTimes = true) => SolvePart(part, 0, displayExecutionTimes);
    public object SolvePart(int part, int testCase, bool displayExecutionTimes = true)
    {
        var methods = new[] { Problem.GetType().GetMethod(SolvePartMethodName(part))! };
        return SolveParts(testCase, methods, displayExecutionTimes)[0];
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
    private static string RunPartMethodName(int part) => ExecutePartMethodName(RunPartMethodPrefix, part);
    private static string ExecutePartMethodName(string prefix, int part) => $"{prefix}{part}";

    private object[] SolveParts(int testCase, MethodInfo[] solutionMethods, bool displayExecutionTimes)
    {
        var result = new object[solutionMethods.Length];

        Problem.CurrentTestCase = testCase;

        var stateLoader = Problem.GetType().GetMethod("LoadState", BindingFlags.NonPublic | BindingFlags.Instance)!;
        bool inputPrints = MethodPrints(stateLoader);
        RunDisplayExecutionTimes(displayExecutionTimes, inputPrints, 0, PrintInputExecutionTime, Problem.EnsureLoadedState);

        for (int i = 0; i < result.Length; i++)
        {
            var method = solutionMethods[i];
            bool prints = MethodPrints(method);
            RunDisplayExecutionTimes(displayExecutionTimes, prints, method.Name.Last().GetNumericValueInteger(), PrintPartExecutionTime, SolveAssignResult);

            void SolveAssignResult()
            {
                result[i] = solutionMethods[i].Invoke(Problem, null)!;
            }
        }
        return result;
    }

    private static bool MethodPrints(MethodInfo method)
    {
        return method.HasCustomAttribute<PrintsToConsoleAttribute>();
    }

    private static void RunDisplayExecutionTimes(bool displayExecutionTimes, bool prints, int part, ExecutionTimeLabelPrinter printer, Action runner)
    {
        bool defaultLivePrintingSetting = ExecutionTimePrinting.EnableLivePrinting;
        if (prints)
            ExecutionTimePrinting.EnableLivePrinting = false;

        if (displayExecutionTimes)
        {
            printer(part);
            ExecutionTimePrinting.BeginExecutionMeasuring();
        }

        runner();

        if (displayExecutionTimes)
        {
            ExecutionTimePrinting.StopExecutionMeasuring().Wait();
        }

        ExecutionTimePrinting.EnableLivePrinting = defaultLivePrintingSetting;
    }

    private delegate void ExecutionTimeLabelPrinter(int part);

    private static void PrintInputExecutionTime(int part)
    {
        ConsoleUtilities.WriteWithColor($"Input".PadLeft(8), ConsoleColor.Cyan);
        Console.Write(':');
    }
    private static void PrintPartExecutionTime(int part)
    {
        ConsoleUtilities.WriteWithColor($"Part ".PadLeft(7), ConsoleColor.Cyan);
        ConsoleUtilities.WriteWithColor(part.ToString(), GetPartColor(part));
        Console.Write(':');
    }

    private static ConsoleColor GetPartColor(int part) => part switch
    {
        1 => ConsoleColor.DarkGray,
        2 => ConsoleColor.DarkYellow,
    };
}
