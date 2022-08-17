using AdventOfCSharp.Extensions;
using AdventOfCSharp.Generation;
using static Garyon.Functions.ConsoleUtilities;
using static System.Console;

namespace AdventOfCSharp;

using static Utilities.ConsolePrinting;

public abstract class BaseProgram
{
    protected static void ValidateAllSolutions()
    {
        WriteLine($"Validating All Problems\n");
        var allProblems = ProblemsIndex.Instance.AllProblems();
        ValidateSolutions(allProblems);
    }

    protected static void ValidateThisYearsSolutions()
    {
        ValidateAllYearSolutions(ServerClock.Now.Year);
    }
    protected static void ValidateTodaysSolution()
    {
        var now = ServerClock.Now;
        ValidateSolution(now.Year, now.Day);
    }

    protected static void ValidateAllYearSolutions(int year)
    {
        Write($"Validating All ");
        DisplayInlineProblemYear(year);
        WriteLine(" Solutions\n");
        ValidateSolutions(ProblemsIndex.Instance.GetYearProblemInfo(year));
    }
    protected static void ValidateSolution(int year, int day)
    {
        Write($"Validating ");
        DisplayInlineProblemDate(year, day);
        WriteLine(" Solution\n");
        ValidateSolution(ProblemsIndex.Instance[year, day]);
    }

    private static void ValidateSolutions(IEnumerable<ProblemInfo> problems)
    {
        var validator = new ProblemValidator();

        foreach (var problem in problems)
            ValidateSolution(problem, validator);

        var report = validator.Report;
        var invalidParts = report.InvalidPartResults.ToArray();
        int validCount = report.ValidPartResults.Count();
        int invalidCount = invalidParts.Length;
        int totalCount = report.AllPartValidationResults.Count();

        bool hasInvalids = validator.Report.HasInvalidParts;
        var invalidPartsColor = hasInvalids ? ConsoleColor.Red : ConsoleColor.Yellow;
        var totalPartsColor = hasInvalids ? ConsoleColor.Yellow : ConsoleColor.Green;

        WriteLineWithColor("  Validation Report", ConsoleColor.Cyan);
        Write("- ");
        WriteLineWithColor($"{validCount  ,3}   valid parts", ConsoleColor.Green);
        Write("- ");
        WriteLineWithColor($"{invalidCount,3} invalid parts", invalidPartsColor);
        Write("- ");
        WriteLineWithColor($"{totalCount  ,3}   total parts", totalPartsColor);

        if (hasInvalids)
        {
            WriteLineWithColor("\nInvalid parts", ConsoleColor.Red);

            foreach (var invalidPart in invalidParts)
            {
                Write("- ");
                DisplayInlineProblemDate(invalidPart.Type.Year, invalidPart.Type.Day, invalidPart.Part, true);
                WriteLine();
            }
        }
        else
        {
            WriteLineWithColor("\nValidation Successful", ConsoleColor.DarkGreen);
        }
    }
    private static void ValidateSolution(ProblemInfo problem, ProblemValidator? validator = null)
    {
        if (problem.HasNoValidSolutions)
            return;

        if (validator is null)
            validator = new ProblemValidator();

        DisplayProblemDate(problem.Year, problem.Day);
        var parts = validator.Validate(problem)!;

        foreach (var result in parts.PartResults)
        {
            if (result.Result is ValidationResult.Invalid)
                WriteLineWithColor($"Part {result.Part} yielded an invalid answer", ConsoleColor.Red);
        }

        WriteLine();
    }

    protected static void EnterMainMenu()
    {
        var problemsIndex = ProblemsIndex.Instance;

        while (true)
        {
            WriteLegend();

            int selectedYear;
            int selectedDay;

            while (true)
            {
                selectedYear = SelectYear();
                selectedDay = SelectDay(selectedYear);

                if (selectedDay != 0)
                    break;
            }

            // Run problem
            WriteLine();
            RunProblem(problemsIndex[selectedYear, selectedDay]!);
        }
    }

    protected static int SelectYear()
    {
        WriteLine("\nAvailable Years:");
        var yearSummary = ProblemsIndex.Instance.GetGlobalYearSummary();
        var availableYears = yearSummary.AvailableYears.ToArray().Sort();
        int minYear = availableYears.First();
        int maxYear = availableYears.Last();
        for (int year = maxYear; year >= minYear; year--)
            WriteSummary(yearSummary[year]);

        WriteLine();
        return ReadConditionalValue(year => yearSummary.Contains(year), "Year ");
    }
    protected static int SelectDay(int selectedYear)
    {
        const int minDay = 1;
        int maxDay = 25;

        var currentDate = ServerClock.Now;

        if (selectedYear == currentDate.Year && currentDate.Month == 12)
            maxDay = Math.Min(maxDay, currentDate.Day);

        WriteLine("\nAvailable Days:");
        var (leftOffset, topOffset) = GetCursorPosition();

        var yearProblemInfo = ProblemsIndex.Instance.GetYearProblemInfo(selectedYear);
        for (int day = minDay; day <= maxDay; day++)
        {
            int column = Math.DivRem(day - 1, 5, out int line);
            SetCursorPosition(leftOffset + column * 7, topOffset + line);

            WriteProblemInfo(selectedYear, day);
        }

        // Useful to ensure the cursor goes in its place
        // after finishing writing the final non-25th day
        SetCursorPosition(0, topOffset + Math.Min(maxDay, 5) + 1);

        WriteLine("Enter 0 to go back to selecting the year.\n");

        return ReadConditionalValue(IsValidDay, "Day  ");

        bool IsValidDay(int day) => day is 0 || yearProblemInfo.Contains(day);
    }

    protected static ConsoleColor ItemColorForAvailability(bool available)
    {
        return available ? ConsoleColor.Cyan : ConsoleColor.DarkGray;
    }
    protected static int ReadConditionalValue(Predicate<int> validator, string? requestMessage = null)
    {
        Write(requestMessage);
        var (left, top) = GetCursorPosition();

        while (true)
        {
            ClearUntilCursorReposition(left, top);

            if (!int.TryParse(ReadLineWithColor(ConsoleColor.Cyan), out int value))
                continue;

            if (validator(value))
                return value;
        }
    }

    protected static void WriteLegend()
    {
        ResetColor();
        WriteLine("Legend:");

        // Valid solutions
        WriteWithColor($"*", GetStatusColor(PartSolutionStatus.Valid));
        Write(" = valid solution (includes ");
        WriteWithColor("unoptimized", GetStatusColor(PartSolutionStatus.Unoptimized));
        WriteLine(" solutions)");

        // Unoptimized solutions
        WriteWithColor("*", GetStatusColor(PartSolutionStatus.Unoptimized));
        WriteLine(" = unoptimized solution");

        // Interactive solutions
        WriteWithColor("*", GetStatusColor(PartSolutionStatus.Interactive));
        WriteLine(" = interactive solution");

        // Refactoring solutions
        WriteWithColor("*", GetStatusColor(PartSolutionStatus.Refactoring));
        WriteLine(" = refactoring solution");

        // WIP solutions
        WriteWithColor("*", GetStatusColor(PartSolutionStatus.WIP));
        Write(" = WIP solution (includes ");
        WriteWithColor("refactoring", GetStatusColor(PartSolutionStatus.Refactoring));
        WriteLine(" solutions)");

        // Uninitialized solutions
        WriteWithColor("*", GetStatusColor(PartSolutionStatus.Uninitialized));
        WriteLine(" = uninitialized solution (empty solution)");

        // Unavailable free stars
        WriteWithColor("*", GetStatusColor(PartSolutionStatus.UnavailableFreeStar));
        WriteLine(" = unavailable free star");
    }

    protected static void WriteProblemInfo(int year, int day)
    {
        var info = ProblemsIndex.Instance[year, day];
        WriteWithColor($" {day,2} ", ItemColorForAvailability(ProblemsIndex.Instance.GetYearProblemInfo(year).Contains(day)));
        WriteStar(info.Part1Status);
        WriteStar(info.Part2Status);
        Write(' ');
    }
    protected static void WriteSummary(YearSummary summary)
    {
        WriteWithColor($"{summary.Year}  ", ItemColorForAvailability(summary.HasAvailableSolutions));
        WriteSummaryStars(summary.StatusCounters.TotalValidSolutions, GetStatusColor(PartSolutionStatus.Valid));
        WriteSummaryStars(summary, PartSolutionStatus.Unoptimized);
        WriteSummaryStars(summary.StatusCounters.TotalWIPSolutions, GetStatusColor(PartSolutionStatus.WIP));
        WriteLine();
    }
    protected static void WriteSummaryStars(YearSummary summary, PartSolutionStatus status)
    {
        WriteSummaryStars(summary.StatusCounters[status], GetStatusColor(status));
    }
    protected static void WriteSummaryStars(int starCount, ConsoleColor starColor)
    {
        WriteStar(starColor);
        var countColor = starColor.Darken();
        WriteWithColor($" {starCount,2}  ", countColor);
    }

    protected static void WriteStar(PartSolutionStatus status)
    {
        WriteStar(GetStatusColor(status));
    }
    protected static void WriteStar(ConsoleColor starColor)
    {
        WriteWithColor("*", starColor);
    }
    protected static ConsoleColor GetStatusColor(PartSolutionStatus status) => status switch
    {
        PartSolutionStatus.Valid => ConsoleColor.DarkYellow,
        PartSolutionStatus.Unoptimized => ConsoleColor.Magenta,
        PartSolutionStatus.WIP => ConsoleColor.Blue,
        PartSolutionStatus.Uninitialized => ConsoleColor.DarkGray,
        PartSolutionStatus.UnavailableFreeStar => ConsoleColor.DarkRed,
        PartSolutionStatus.Refactoring => ConsoleColor.Cyan,
        PartSolutionStatus.Interactive => ConsoleColor.DarkGreen,
        _ => ConsoleColor.White,
    };

    protected static void RunTodaysProblem(bool testCases = true)
    {
        var currentDate = ServerClock.Now;
        var currentYear = currentDate.Year;
        var currentDay = currentDate.Day;

        if (!RunProblem(currentYear, currentDay, testCases))
        {
            WriteLine($@"
It seems today's problem has no solution class
Focus on development, you lazy fucking ass
              --A happy AoC solver, to himself
");

            if (SolutionTemplateGeneration.EnabledGeneration)
            {
                SolutionTemplateGeneration.CreateSolutionFile(currentYear, currentDay);
                WriteLine("The solution file for today's problem has been automatically created.");
            }
        }
    }

    protected static bool RunThisYearsProblem(int day, bool testCases = true)
    {
        var currentDate = ServerClock.Now;
        var currentYear = currentDate.Year;
        return RunProblem(currentYear, day, testCases);
    }
    protected static bool RunProblem(int year, int day, bool testCases = true)
    {
        return RunProblem(ProblemsIndex.Instance[year, day], testCases);
    }

    protected static bool RunProblem<T>()
        where T : Problem, new()
    {
        return RunProblem(typeof(T));
    }
    protected static bool RunProblem(ProblemInfo problemInfo, bool testCases = true)
    {
        return RunProblem(problemInfo.ProblemType.ProblemClass, testCases);
    }
    protected static bool RunProblem(Type problemType, bool testCases = true)
    {
        var instance = problemType?.GetConstructor(Type.EmptyTypes)!.Invoke(null);
        if (instance is not Problem problem)
            return false;

        RunProblemWithTestCases(problem, testCases);
        return true;
    }
    protected static void RunProblemWithTestCases(Problem instance, bool testCases)
    {
        if (testCases)
            RunProblemTestCases(instance);
        RunProblem(instance);
    }
    protected static void RunProblem(Problem instance)
    {
        RunProblemCase(instance, 0);
    }
    protected static void RunProblemTestCases(Problem instance)
    {
        foreach (int testCase in instance.Input.TestCaseIDs)
            RunProblemCase(instance, testCase);
    }

    protected static void RunProblemCase(Problem instance, int testCase)
    {
        DisplayProblemDate(instance.Year, instance.Day);
        Write("Running ");
        if (testCase is 0)
            WriteLineWithColor("main problem\n", ConsoleColor.Green);
        else
        {
            WriteWithColor("test case ", ConsoleColor.DarkYellow);
            WriteLineWithColor($"{testCase}\n", ConsoleColor.Cyan);
        }

        var parts = new ProblemRunner(instance).SolveAllParts(testCase);
        WriteLine();
        foreach (var part in parts)
        {
            FancyPrinting.PrintLabel(part.PartName);
            WriteLineWithColor($" {AnswerStringConversion.Convert(part.Output)}", GetAnswerColor(instance, part.PartName));
        }
        WriteLine();
    }

    private static ConsoleColor GetAnswerColor(Problem instance, string partName)
    {
        if (IsFinalDayPart2(instance, partName))
        {
            return IFinalDay.IsAvailable(instance.Year) switch
            {
                true => ConsoleColor.Green,
                false => ConsoleColor.Red,
            };
        }

        return ConsoleColor.Yellow;
    }
    private static bool IsFinalDayPart2(Problem instance, string partName)
    {
        return partName is "Part 2" && instance is IFinalDay;
    }

    private static void DisplayInlineProblemStat(string label, int stat, int minWidth = 1)
    {
        Write(label);
        WriteWithColor(stat.ToString().PadLeft(minWidth), ConsoleColor.Cyan);
    }
    protected static void DisplayInlineProblemYear(int year)
    {
        DisplayInlineProblemStat("Year ", year);
    }
    protected static void DisplayInlineProblemDay(int day, bool pad = false)
    {
        int minWidth = pad ? 2 : 1;
        DisplayInlineProblemStat(" Day ", day, minWidth);
    }
    protected static void DisplayInlineProblemPart(int part)
    {
        DisplayInlineProblemStat(" Part ", part);
    }
    protected static void DisplayInlineProblemDate(int year, int day, bool padDay = false)
    {
        DisplayInlineProblemYear(year);
        DisplayInlineProblemDay(day, padDay);
    }
    protected static void DisplayInlineProblemDate(int year, int day, int part, bool padDay = false)
    {
        DisplayInlineProblemDate(year, day, padDay);
        DisplayInlineProblemPart(part);
    }
    protected static void DisplayProblemDate(int year, int day)
    {
        WriteWithColor("Year ", ConsoleColor.DarkRed, false);
        WriteWithColor("20", ConsoleColor.DarkYellow, false);
        WriteLineWithColor((year % 100).ToString(), ConsoleColor.DarkGreen, false);

        WriteWithColor("Day  ", ConsoleColor.DarkRed, false);
        WriteLineWithColor(day.ToString().PadLeft(4), ConsoleColor.DarkGreen);
    }
}
