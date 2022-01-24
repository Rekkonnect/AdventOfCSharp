using System.Text.RegularExpressions;

namespace AdventOfCSharp;

public static class FancyPrinting
{
    private static readonly Regex partPattern = new(@"Part (\d*)$");

    public static void PrintLabel(string partName)
    {
        GetPartLabelPrinter(partName)(partName);
    }
    public static PartLabelPrinter GetPartLabelPrinter(string partName)
    {
        var match = partPattern.Match(partName);

        if (match.Success)
            return PrintPartLabel;

        return PrintCustomPartLabel;
    }

    public delegate void PartLabelPrinter(string partName);

    public static void PrintCustomPartLabel(string partName)
    {
        if (partName.Length > 20)
            partName = partName[..20];

        ConsoleUtilities.WriteWithColor(partName.PadLeft(20), ConsoleColor.Cyan);
        Console.Write(':');
    }
    public static void PrintPartLabel(string partName)
    {
        int part = GetOfficialPartIndex(partName);
        var partString = part.ToString();
        var partPrefix = partName[..^partString.Length];
        ConsoleUtilities.WriteWithColor(partPrefix.PadLeft(20 - partString.Length), ConsoleColor.Cyan);
        ConsoleUtilities.WriteWithColor(partString, GetPartColor(part));
        Console.Write(':');
    }

    private static int GetOfficialPartIndex(string partName)
    {
        // Part indices >= 10 aren't supposed to exist
        return partName.Last().GetNumericValueInteger();
    }

    private static ConsoleColor GetPartColor(int part) => part switch
    {
        1 => ConsoleColor.DarkGray,
        2 => ConsoleColor.DarkYellow,

        // This will catch some users off-guard
        3 => ConsoleColor.DarkRed,
        > 3 => ConsoleColor.Magenta,

        // "Where did my 0 go?"
        _ => Console.ForegroundColor,
    };
}
