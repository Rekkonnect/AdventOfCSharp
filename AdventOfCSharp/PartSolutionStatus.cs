namespace AdventOfCSharp;

/// <summary>Denotes a part's solution status.</summary>
public enum PartSolutionStatus
{
    /// <summary>The solution has not begun yet.</summary>
    Uninitialized,

    /// <summary>The solution is in progress.</summary>
    WIP,
    /// <summary>The solution is valid, but unoptimized.</summary>
    /// <remarks>This value should be used for solutions whose execution time is unexpectedly long, or above 10-15 seconds.</remarks>
    Unoptimized,

    /// <summary>The solution is valid.</summary>
    Valid,

    /// <summary>
    /// Represents a free star that is not currently available.
    /// This is (so far) only the case for D25P2 if not all other 49 star have been claimed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    UnavailableFreeStar,

    /// <summary>The solution is under a refactoring process.</summary>
    /// <remarks>
    /// This value implies that the part has been previously solved, but the implementation is being refactored.
    /// Consider using <seealso cref="WIP"/> if the part has not been solved in a previous timeframe.
    /// </remarks>
    Refactoring,
}

public static class PartSolutionStatusExtensions
{
    public static bool IsValidSolution(this PartSolutionStatus status) => status is PartSolutionStatus.Valid or PartSolutionStatus.Unoptimized;
    public static bool HasBeenSolved(this PartSolutionStatus status) => IsValidSolution(status) || status is PartSolutionStatus.Refactoring;
}