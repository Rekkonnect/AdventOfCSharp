namespace AdventOfCSharp;

using static Utilities.ConsolePrinting;

public static class ConsoleBufferHandling
{
    /// <summary>
    /// Gets or sets the current <seealso cref="BufferHeightHandlingMode"/>
    /// that will be used when setting the cursor position to heights
    /// beyond the currently available on the buffer.
    /// </summary>
    public static BufferHeightHandlingMode BufferHeightHandlingMode { get; set; }

    static ConsoleBufferHandling()
    {
        BufferHeightHandlingMode = DefaultBufferHeightHandlingModeForCurrentPlatform();
    }

    public static void PrepareAccessingCursorTop(int lines)
    {
        int top = Console.CursorTop + lines;
        AccessCursorTop(top);
    }

    public static void AccessCursorTop(int top)
    {
        if (top < 0)
            throw new ArgumentOutOfRangeException(nameof(top), "The cursor top may not be a negative value.");

        int missingLines = top - Console.BufferHeight + 1;
        if (missingLines <= 0)
            return;

        switch (BufferHeightHandlingMode)
        {
            case BufferHeightHandlingMode.None:
                break;

            case BufferHeightHandlingMode.MaximizeBufferHeight:
                Console.BufferHeight = short.MaxValue;
                break;

            case BufferHeightHandlingMode.ClearBuffer:
                Console.Clear();
                break;

            case BufferHeightHandlingMode.PrintNewlines:
                int previousLeft = Console.CursorLeft;
                WriteNewLines(missingLines);
                Console.CursorTop -= missingLines;
                Console.CursorLeft = previousLeft;
                break;
        }
    }

    /// <summary>
    /// Gets the default <seealso cref="BufferHeightHandlingMode"/> for
    /// the current platform.
    /// </summary>
    /// <returns>
    /// <seealso cref="BufferHeightHandlingMode.PrintNewlines"/> for all
    /// platforms; subject to change in the future depending on feedback
    /// and system-wide adjustments.
    /// </returns>
    public static BufferHeightHandlingMode DefaultBufferHeightHandlingModeForCurrentPlatform()
    {
        var operatingSystem = Environment.OSVersion;

        switch (operatingSystem.Platform)
        {
            case PlatformID.Win32NT:
            case PlatformID.Unix:
            case PlatformID.Other:
                return BufferHeightHandlingMode.PrintNewlines;

            default:
                return BufferHeightHandlingMode.None;
        }
    }
}
