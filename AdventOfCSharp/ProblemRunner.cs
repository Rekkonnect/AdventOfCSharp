using System.Reflection.Metadata.Ecma335;

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

    public static ProblemRunner ForProblem(int year, int day) => new(ProblemsIndex.Instance[year, day].InitializeInstance());

    public object[] SolveAllParts(bool displayExecutionTimes = true) => SolveAllParts(0, displayExecutionTimes);
    public object[] SolveAllParts(int testCase, bool displayExecutionTimes = true)
    {
        var methods = Problem.GetType().GetMethods().Where(m => m.Name.StartsWith(SolvePartMethodPrefix)).ToArray();
        return SolveParts(testCase, methods, displayExecutionTimes);
    }

    public object SolvePart(int part, bool displayExecutionTimes = true) => SolvePart(part, 0, displayExecutionTimes);
    public object SolvePart(int part, int testCase, bool displayExecutionTimes = true)
    {
        var methods = new[] { Problem.GetType().GetMethod(SolvePartMethodName(part)) };
        return SolveParts(testCase, methods, displayExecutionTimes)[0];
    }

    public bool FullyValidateAllTestCases()
    {
        for (int i = 0; i <= Problem.TestCaseFiles; i++)
        {
            if (!ValidateAllParts(i))
                return false;
        }

        return true;
    }
    public bool ValidateAllParts()
    {
        return ValidateAllParts(0);
    }
    public bool ValidateAllParts(int testCase)
    {
        return ValidatePart(1, testCase) && ValidatePart(2, testCase);
    }

    public bool ValidatePart(int part) => ValidatePart(part, 0);
    public bool ValidatePart(int part, int testCase)
    {
        var contents = Problem.GetOutputFileContents(testCase, true);
        var expectedPartOutput = contents.ForPart(part);
        if (expectedPartOutput is null)
            return true;

        return ValidatePart(part, testCase, expectedPartOutput);
    }
    private bool ValidatePart(int part, int testCase, string expected)
    {
        return expected.Equals(AnswerStringConversion.Convert(SolvePart(part, testCase)), StringComparison.OrdinalIgnoreCase);
    }

    private static string SolvePartMethodName(int part) => ExecutePartMethodName(SolvePartMethodPrefix, part);
    private static string RunPartMethodName(int part) => ExecutePartMethodName(RunPartMethodPrefix, part);
    private static string ExecutePartMethodName(string prefix, int part) => $"{prefix}{part}";

    private object[] SolveParts(int testCase, MethodInfo[] solutionMethods, bool displayExecutionTimes)
    {
        var result = new object[solutionMethods.Length];

        Problem.CurrentTestCase = testCase;
        DisplayExecutionTimes(displayExecutionTimes, 0, PrintInputExecutionTime, Problem.EnsureLoadedState);

        for (int i = 0; i < result.Length; i++)
        {
            DisplayExecutionTimes(displayExecutionTimes, solutionMethods[i].Name.Last().GetNumericValueInteger(), PrintPartExecutionTime, SolveAssignResult);

            void SolveAssignResult()
            {
                result[i] = solutionMethods[i].Invoke(Problem, null)!;
            }
        }
        return result;
    }

    private static void DisplayExecutionTimes(bool displayExecutionTimes, int part, ExecutionTimePrinter printer, Action action)
    {
        var executionTime = BasicBenchmarking.MeasureExecutionTime(action);

        if (displayExecutionTimes)
            printer(part, executionTime);
    }

    private delegate void ExecutionTimePrinter(int part, TimeSpan executionTime);

    private static void PrintInputExecutionTime(int part, TimeSpan executionTime)
    {
        ConsoleUtilities.WriteWithColor($"Input".PadLeft(8), ConsoleColor.Cyan);
        PrintExecutionTime(executionTime);
    }
    private static void PrintPartExecutionTime(int part, TimeSpan executionTime)
    {
        ConsoleUtilities.WriteWithColor($"Part ".PadLeft(7), ConsoleColor.Cyan);
        ConsoleUtilities.WriteWithColor(part.ToString(), GetPartColor(part));
        PrintExecutionTime(executionTime);
    }

    private static ConsoleColor GetPartColor(int part) => part switch
    {
        1 => ConsoleColor.DarkGray,
        2 => ConsoleColor.DarkYellow,
    };

    private static void PrintExecutionTime(TimeSpan executionTime)
    {
        Console.Write(':');
        ConsoleUtilities.WriteLineWithColor($"{executionTime.TotalMilliseconds,13:N2} ms", GetExecutionTimeColor(executionTime));
    }
    private static ConsoleColor GetExecutionTimeColor(TimeSpan executionTime) => executionTime.TotalMilliseconds switch
    {
        < 1 => ConsoleColor.Blue,
        < 5 => ConsoleColor.Cyan,
        < 20 => ConsoleColor.Green,
        < 100 => ConsoleColor.DarkGreen,
        < 400 => ConsoleColor.Yellow,
        < 1000 => ConsoleColor.DarkYellow,
        < 3000 => ConsoleColor.Magenta,
        < 15000 => ConsoleColor.Red,
        _ => ConsoleColor.DarkRed,
    };
}
