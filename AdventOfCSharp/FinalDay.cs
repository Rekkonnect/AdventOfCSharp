namespace AdventOfCSharp;

public interface IFinalDay
{
    internal const string LockedPartString = "You have not collected all 49* to unlock this part!";

    private protected static string GetCompletionCongratulationString(int year) => $"Congratulations on completing all of AoC {year}!";

    public static bool IsAvailable(int year)
    {
        return ProblemsIndex.Instance.DetermineLastDayPart2Availability(year);
    }
}

public abstract class FinalDay<T> : Problem<T, string>, IFinalDay
    where T : notnull
{
    public sealed override string SolvePart2()
    {
        if (!IFinalDay.IsAvailable(Year))
            return IFinalDay.LockedPartString;

        return IFinalDay.GetCompletionCongratulationString(Year);
    }
}
