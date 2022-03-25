using RoseLynn;

namespace AdventOfCSharp.CodeAnalysis.Core;

public static class KnownSymbolNames
{
    public const string FinalDay = nameof(FinalDay);

    public const string Problem = nameof(Problem);

    public const string SolvePart1 = nameof(SolvePart1);
    public const string SolvePart2 = nameof(SolvePart2);

    public const string CommonAnswerStringConverter = nameof(CommonAnswerStringConverter);
    public const string AnswerStringConverter = nameof(AnswerStringConverter);

    public const string BenchmarkDescriber = nameof(BenchmarkDescriber);
}

public static class KnownFullSymbolNames
{
    private static readonly string[] BaseAoCSNamespaces = new[] { nameof(AdventOfCSharp) };
    private static readonly string[] BaseAoCSBenchmarkingNamespaces = new[] { nameof(AdventOfCSharp), nameof(AdventOfCSharp.Benchmarking) };

    // AoCS
    public static readonly FullSymbolName CommonAnswerStringConverter =
        new(KnownSymbolNames.CommonAnswerStringConverter, BaseAoCSNamespaces);

    public static readonly FullSymbolName GenericAnswerStringConverter =
        new(new IdentifierWithArity(KnownSymbolNames.AnswerStringConverter, 1), BaseAoCSNamespaces);

    public static readonly FullSymbolName NonGenericAnswerStringConverter =
        new(KnownSymbolNames.AnswerStringConverter, BaseAoCSNamespaces);

    // AoCS.Benchmarking
    public static readonly FullSymbolName BenchmarkDescriber =
        new(KnownSymbolNames.BenchmarkDescriber, BaseAoCSBenchmarkingNamespaces);
}
