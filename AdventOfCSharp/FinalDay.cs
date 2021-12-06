namespace AdventOfCSharp;

public abstract class FinalDay<T> : Problem<T, string>
{
    public sealed override string SolvePart2()
    {
        if (!ProblemsIndex.Instance.DetermineLastDayPart2Availability(Year))
            return "You have not collected all 49* to unlock this part!";

        return $"Congratulations on completing all of AoC {Year}!";
    }
}
