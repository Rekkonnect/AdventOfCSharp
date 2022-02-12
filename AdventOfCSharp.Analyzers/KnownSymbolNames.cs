using RoseLynn;

namespace AdventOfCSharp.Analyzers;

public static class KnownSymbolNames
{
    public const string PartSolverAttribute = nameof(PartSolverAttribute);
    public const string PartSolutionAttribute = nameof(PartSolutionAttribute);

    public const string SecretsContainerAttribute = nameof(SecretsContainerAttribute);

    public const string FinalDay = nameof(FinalDay);

    public const string Problem = nameof(Problem);
    public const string SecretsContainer = nameof(SecretsContainer);
    public const string ISecretsContainer = nameof(ISecretsContainer);

    public const string SolvePart1 = nameof(SolvePart1);
    public const string SolvePart2 = nameof(SolvePart2);

    public const string CommonAnswerStringConverter = nameof(CommonAnswerStringConverter);
    public const string AnswerStringConverter = nameof(AnswerStringConverter);
}

public static class KnownFullSymbolNames
{
    private static readonly string[] BaseAoCSNamespaces = new[] { nameof(AdventOfCSharp) };

    public static readonly FullSymbolName CommonAnswerStringConverter =
        new(KnownSymbolNames.CommonAnswerStringConverter, BaseAoCSNamespaces);

    public static readonly FullSymbolName GenericAnswerStringConverter =
        new(new IdentifierWithArity(KnownSymbolNames.AnswerStringConverter, 1), BaseAoCSNamespaces);

    public static readonly FullSymbolName NonGenericAnswerStringConverter =
        new(KnownSymbolNames.AnswerStringConverter, BaseAoCSNamespaces);
}
