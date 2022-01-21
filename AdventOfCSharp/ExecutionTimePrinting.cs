using System.Diagnostics;
using System.Threading.Tasks;

namespace AdventOfCSharp;

/// <summary>Provides mechanisms for printing execution times.</summary>
public static class ExecutionTimePrinting
{
    private static readonly object executionTimeLock = new();

    /// <summary>Determines whether printing the current execution time is enabled.</summary>
    public static bool EnableLivePrinting { get; set; } = true;

    private static int cursorTop, cursorLeft;
    private static Stopwatch currentExecutionTime = new();
    private static Task? livePrintingTask = null;

    /// <summary>Begins execution measuring, while also printing the live execution time if <seealso cref="EnableLivePrinting"/> is <see langword="true"/>.</summary>
    public static void BeginExecutionMeasuring()
    {
        RegisterCurrentCursorPosition();
        currentExecutionTime = new();
        currentExecutionTime.Start();

        if (EnableLivePrinting)
        {
            livePrintingTask = LoopPrintAsync();
        }
    }

    private static async Task LoopPrintAsync()
    {
        while (currentExecutionTime.IsRunning)
        {
            PrintCurrentExecutionTime();
            // Don't stress the lock too much
            await Task.Delay(1);
        }
    }
    private static void PrintCurrentExecutionTime()
    {
        lock (executionTimeLock)
        {
            Console.SetCursorPosition(cursorLeft, cursorTop);
            PrintExecutionTime(currentExecutionTime.Elapsed);
        }
    }

    /// <summary>Stops execution measuring, while also printing the final execution time. <seealso cref="EnableLivePrinting"/> is ignored.</summary>
    public static async Task StopExecutionMeasuring()
    {
        currentExecutionTime.Stop();
        PrintCurrentExecutionTime();

        if (livePrintingTask is not null)
        {
            await livePrintingTask;
            livePrintingTask = null;
        }
    }

    private static void RegisterCurrentCursorPosition()
    {
        (cursorLeft, cursorTop) = Console.GetCursorPosition();
    }

    /// <summary>Prints the given execution time, with the respective color.</summary>
    /// <param name="executionTime">The execution time to print.</param>
    public static void PrintExecutionTime(TimeSpan executionTime)
    {
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
