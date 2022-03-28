namespace AdventOfCSharp.Utilities;

public static class ConsoleAvailability
{
    private static readonly Lazy<bool> hasInput = new(GetHasInput);
    private static readonly Lazy<bool> supportsCursorPosition = new(GetSupportsCursorPosition);

    public static bool HasInput => hasInput.Value;
    public static bool SupportsCursorPosition => supportsCursorPosition.Value;

    // Exception-driven programming pog
    private static bool GetHasInput()
    {
        try
        {
            _ = Console.In;
            return true;
        }
        catch
        {
            return false;
        }
    }
    private static bool GetSupportsCursorPosition()
    {
        try
        {
            _ = Console.CursorLeft;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
