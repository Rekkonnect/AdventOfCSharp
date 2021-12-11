using AdventOfCSharp.Extensions;
using static Garyon.Functions.ConsoleUtilities;
using static System.Console;

namespace AdventOfCSharp;

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

    protected static void ValidateSolutions(IEnumerable<ProblemInfo> problems)
    {
        foreach (var problem in problems)
            ValidateSolution(problem);
    }
    protected static void ValidateSolution(ProblemInfo problem)
    {
        var instance = problem.InitializeInstance();
        if (instance is null)
            return;

        var runner = new ProblemRunner(instance);

        DisplayProblemDate(problem.Year, problem.Day);
        ValidatePart(1);
        ValidatePart(2);
        WriteLine();

        void ValidatePart(int part)
        {
            if (problem.StatusForPart(part) is not PartSolutionStatus.Valid)
                return;
            if (!runner.ValidatePart(part))
                WriteLineWithColor($"Part {part} yielded an invalid answer", ConsoleColor.Red);
        }
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
            maxDay = currentDate.Day;

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

    #region Pretty console writing
    // This kind of functionality must be available somewhere
    // I should browse some packages
    protected static void ClearUntilCursorReposition(int startLeft, int startTop)
    {
        ClearUntilCursor(startLeft, startTop);
        SetCursorPosition(startLeft, startTop);
    }
    protected static void ClearUntilCursor(int startLeft, int startTop)
    {
        int length = GetConsoleBufferDifference(startLeft, startTop);

        CursorTop = startTop;
        CursorLeft = startLeft;

        var clearString = new string(' ', length);
        Write(clearString);
    }
    protected static int GetConsoleBufferDifference(int startLeft, int startTop)
    {
        var (endLeft, endTop) = GetCursorPosition();
        return GetConsoleBufferDifference(startLeft, startTop, endLeft, endTop);
    }
    protected static int GetConsoleBufferDifference(int startLeft, int startTop, int endLeft, int endTop)
    {
        int width = BufferWidth;
        int differenceLeft = endLeft - startLeft;
        int differenceTop = endTop - startTop;
        return differenceTop * width - differenceLeft;
    }
    #endregion

    protected static void WriteLegend()
    {
        ResetColor();
        WriteLine("Legend:");

        // Valid solutions
        WriteWithColor($"*", GetStarColor(PartSolutionStatus.Valid));
        Write(" = valid solution (includes ");
        WriteWithColor("unoptimized", GetStarColor(PartSolutionStatus.Unoptimized));
        WriteLine(" solutions)");

        // Unoptimized solutions
        WriteWithColor("*", GetStarColor(PartSolutionStatus.Unoptimized));
        WriteLine(" = unoptimized solution");

        // WIP solutions
        WriteWithColor("*", GetStarColor(PartSolutionStatus.WIP));
        WriteLine(" = WIP solution");

        // Uninitialized solutions
        WriteWithColor("*", GetStarColor(PartSolutionStatus.Uninitialized));
        WriteLine(" = uninitialized solution (empty solution)");

        // Unavailable free stars
        WriteWithColor("*", GetStarColor(PartSolutionStatus.UnavailableFreeStar));
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
        WriteSummaryStars(summary.StatusCounters.TotalValidSolutions, GetStarColor(PartSolutionStatus.Valid));
        WriteSummaryStars(summary, PartSolutionStatus.Unoptimized);
        WriteSummaryStars(summary, PartSolutionStatus.WIP);
        WriteLine();
    }
    protected static void WriteSummaryStars(YearSummary summary, PartSolutionStatus status)
    {
        WriteSummaryStars(summary.StatusCounters[status], GetStarColor(status));
    }
    protected static void WriteSummaryStars(int starCount, ConsoleColor starColor)
    {
        WriteStar(starColor);
        var countColor = starColor.Darken();
        WriteWithColor($" {starCount,2}  ", countColor);
    }

    protected static void WriteStar(PartSolutionStatus status)
    {
        WriteStar(GetStarColor(status));
    }
    protected static void WriteStar(ConsoleColor starColor)
    {
        WriteWithColor("*", starColor);
    }
    protected static ConsoleColor GetStarColor(PartSolutionStatus status) => status switch
    {
        PartSolutionStatus.Valid => ConsoleColor.DarkYellow,
        PartSolutionStatus.Unoptimized => ConsoleColor.Magenta,
        PartSolutionStatus.WIP => ConsoleColor.Blue,
        PartSolutionStatus.Uninitialized => ConsoleColor.DarkGray,
        PartSolutionStatus.UnavailableFreeStar => ConsoleColor.DarkRed,
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
        var instance = problemType?.GetConstructor(Type.EmptyTypes)!.Invoke(null) as Problem;
        if (instance is null)
            return false;

        RunProblemWithTestCases(instance, testCases);
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
        int testCases = instance.TestCaseFiles;
        for (int i = 1; i <= testCases; i++)
            RunProblemCase(instance, i);
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
            WriteLine(AnswerStringConversion.Convert(part));
        WriteLine();
    }

    protected static void DisplayInlineProblemYear(int year)
    {
        Write("Year ");
        WriteWithColor(year.ToString(), ConsoleColor.Cyan);
    }
    protected static void DisplayInlineProblemDay(int day)
    {
        Write(" Day ");
        WriteWithColor(day.ToString(), ConsoleColor.Cyan);
    }
    protected static void DisplayInlineProblemDate(int year, int day)
    {
        DisplayInlineProblemYear(year);
        DisplayInlineProblemDay(day);
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
