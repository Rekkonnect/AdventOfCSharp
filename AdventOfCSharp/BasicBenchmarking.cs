namespace AdventOfCSharp;

public static class BasicBenchmarking
{
    public static TimeSpan MeasureExecutionTime(Action action)
    {
        var start = DateTime.Now;
        action();
        var end = DateTime.Now;
        return end - start;
    }
}
