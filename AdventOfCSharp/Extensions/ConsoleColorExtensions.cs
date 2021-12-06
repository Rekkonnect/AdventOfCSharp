namespace AdventOfCSharp.Extensions;

public static class ConsoleColorExtensions
{
    public static ConsoleColor Darken(this ConsoleColor color) => color switch
    {
        ConsoleColor.Gray => ConsoleColor.DarkGray,
        >= ConsoleColor.DarkGray => color - 8,
        _ => color,
    };
    public static ConsoleColor Lighten(this ConsoleColor color) => color switch
    {
        ConsoleColor.DarkGray => ConsoleColor.Gray,
        <= ConsoleColor.Gray => color + 8,
        _ => color,
    };
}
