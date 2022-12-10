using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Console;

namespace AdventOfCSharp.Utilities;

public static class ConsolePrinting
{
    // This kind of functionality must be available somewhere
    // I should browse some packages
    public static void ClearUntilCursorReposition(int startLeft, int startTop)
    {
        ClearUntilCursor(startLeft, startTop);
        SetCursorPosition(startLeft, startTop);
    }
    public static void ClearUntilCursor(int startLeft, int startTop)
    {
        int length = GetConsoleBufferDifference(startLeft, startTop);

        CursorTop = startTop;
        CursorLeft = startLeft;

        var clearString = new string(' ', length);
        Write(clearString);
    }
    public static int GetConsoleBufferDifference(int startLeft, int startTop)
    {
        var (endLeft, endTop) = GetCursorPosition();
        return GetConsoleBufferDifference(startLeft, startTop, endLeft, endTop);
    }
    public static int GetConsoleBufferDifference(int startLeft, int startTop, int endLeft, int endTop)
    {
        int width = BufferWidth;
        int differenceLeft = endLeft - startLeft;
        int differenceTop = endTop - startTop;
        return differenceTop * width - differenceLeft;
    }

    public static void WriteNewLines(int newlineCount)
    {
        var newline = Environment.NewLine;
        var builder = new StringBuilder(newline.Length * newlineCount);
        for (int i = 0; i < newlineCount; i++)
        {
            builder.Append(newline);
        }

        Write(builder.ToString());
    }
}
