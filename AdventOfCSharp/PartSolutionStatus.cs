namespace AdventOfCSharp;

public enum PartSolutionStatus
{
    Uninitialized,

    WIP,
    Unoptimized,

    Valid,

    /// <summary>
    /// Represents a free star that is not currently available.
    /// This is (so far) only the case for D25P2 if not all other 49 star have been claimed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    UnavailableFreeStar,

    Refactoring,
}

public static class PartSolutionStatusExtensions
{
    public static bool IsValidSolution(this PartSolutionStatus status) => status is PartSolutionStatus.Valid or PartSolutionStatus.Unoptimized;
    public static bool HasBeenSolved(this PartSolutionStatus status) => IsValidSolution(status) || status is PartSolutionStatus.Refactoring;
}