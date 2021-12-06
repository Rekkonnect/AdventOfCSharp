namespace AdventOfCSharp;

public static class ServerClock
{
    /// <summary>Returns the current time adjusted to UTC-5, the timezone the server operates on.</summary>
    /// <remarks>
    /// "Current time" refers to this system's time. There is currently no proper way to get the exact time on the server.
    /// High clock precision barely matters in this project. In most scenarios, server responses contain meaningful information
    /// regarding its server time, over the form of durations, intervals or remaining spans.
    /// </remarks>
    public static DateTime Now => DateTime.UtcNow - TimeSpan.FromHours(5);
}
