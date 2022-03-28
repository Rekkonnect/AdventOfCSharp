using AdventOfCSharp.Utilities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AdventOfCSharp.Benchmarking")]

namespace AdventOfCSharp;

/// <summary>Provides a collection of options for running <seealso cref="Problem"/> instances through <seealso cref="ProblemRunner"/>.</summary>
public sealed class ProblemRunningOptions
{
    public static ProblemRunningOptions Default => new();

    /// <summary>Determines whether execution times will be displayed.</summary>
    public bool DisplayExecutionTimes { get; set; } = true;
}

/// <summary>Provides mechanisms for running problem solutions.</summary>
public sealed class ProblemRunner
{
    /// <summary>The problem instance that is being run.</summary>
    public Problem Problem { get; }

    /// <summary>Gets the <seealso cref="Type"/> of the given <seealso cref="AdventOfCSharp.Problem"/> instance.</summary>
    public Type ProblemType => Problem.GetType();

    /// <summary>Gets the options for running the current <seealso cref="AdventOfCSharp.Problem"/> instance.</summary>
    public ProblemRunningOptions Options { get; }

    public ProblemRunner(Problem problem)
        : this(problem, null) { }
    public ProblemRunner(Problem problem, ProblemRunningOptions? options)
    {
        Problem = problem;
        Options = options ?? ProblemRunningOptions.Default;

        DisablePrintingIfUnavailable();
    }

    private void DisablePrintingIfUnavailable()
    {
        if (!ConsoleAvailability.SupportsCursorPosition)
            Options.DisplayExecutionTimes = false;
    }

    private static ProblemRunner? ForInstance(Problem? instance)
    {
        if (instance is null)
            return null;

        return new(instance);
    }

    /// <summary>Creates a new <seealso cref="ProblemRunner"/> instance for the problem of the specified day.</summary>
    /// <param name="year">The year of the problem.</param>
    /// <param name="day">The day of the problem.</param>
    /// <returns>A <seealso cref="ProblemRunner"/> instance for the specified problem, if a solution class is available for it, otherwise <see langword="null"/>.</returns>
    public static ProblemRunner? ForProblem(int year, int day) => ForInstance(ProblemsIndex.Instance[year, day].InitializeInstance());

    public PartSolutionOutputDictionary SolveAllParts() => SolveAllParts(0);
    public PartSolutionOutputDictionary SolveAllParts(int testCase)
    {
        return SolveParts(testCase, ProblemSolverMethodProvider.PartSolverMethods(ProblemType));
    }

    public PartSolutionOutputDictionary SolveAllOfficialParts() => SolveAllOfficialParts(0);
    public PartSolutionOutputDictionary SolveAllOfficialParts(int testCase)
    {
        return SolveParts(testCase, ProblemSolverMethodProvider.MethodsForOfficialParts(ProblemType));
    }

    public object SolvePart(int part) => SolvePart(part, 0);
    public object SolvePart(int part, int testCase)
    {
        var methods = new[] { ProblemSolverMethodProvider.MethodForPart(ProblemType, part) };
        return SolveParts(testCase, methods).GetPartOutput(part)!;
    }

    public ValidationProblemResult FullyValidateAllTestCases()
    {
        var results = new List<ValidationPartResult>();

        foreach (int testCase in Problem.Input.TestCaseIDs)
        {
            var caseResult = ValidateAllParts(testCase);
            results.AddRange(caseResult.PartResults);
        }

        return new(results);
    }

    public ValidationProblemResult ValidateAllParts()
    {
        return ValidateAllParts(0);
    }
    public ValidationProblemResult ValidateAllParts(int testCase)
    {
        var validations = new[] { ValidatePart(1, testCase), ValidatePart(2, testCase) };
        return new(validations.Where(Predicates.NotNull) as IEnumerable<ValidationPartResult>);
    }

    public ValidationPartResult? ValidatePart(int part) => ValidatePart(part, 0);
    public ValidationPartResult? ValidatePart(int part, int testCase)
    {
        var contents = Problem.Output.GetOutputFileContents(testCase, true);
        var expectedPartOutput = contents.ForPart(part);
        if (expectedPartOutput is null)
            return null;

        return ValidatePart(part, testCase, expectedPartOutput);
    }
    private ValidationPartResult ValidatePart(int part, int testCase, string expected)
    {
        bool valid = false;
        try
        {
            valid = expected.Equals(AnswerStringConversion.Convert(SolvePart(part, testCase)), StringComparison.OrdinalIgnoreCase);
        }
        catch { }

        var result = valid switch
        {
            true => ValidationResult.Valid,
            false => ValidationResult.Invalid,
        };

        return new(ProblemsIndex.Instance.InfoForInstance(Problem).ProblemType, part, testCase, result);
    }

    private PartSolutionOutputDictionary SolveParts(int testCase, MethodInfo[] solutionMethods)
    {
        var result = new PartSolutionOutputDictionary();

        Problem.CurrentTestCase = testCase;

        if (!Problem.StateLoaded)
        {
            RunDisplayExecutionTimes(false, "Download", FancyPrinting.PrintCustomPartLabel, Problem.EnsureDownloadedInput);

            var stateLoader = ProblemSolverMethodProvider.LoadStateMethod(ProblemType);
            bool inputPrints = MethodPrints(stateLoader);
            RunDisplayExecutionTimes(inputPrints, "Input", FancyPrinting.PrintCustomPartLabel, Problem.EnsureLoadedState);
        }

        for (int i = 0; i < solutionMethods.Length; i++)
        {
            var method = solutionMethods[i];
            bool prints = MethodPrints(method);
            var partName = method.GetCustomAttribute<PartSolverAttribute>()!.PartName;

            RunDisplayExecutionTimes(prints, partName, FancyPrinting.GetPartLabelPrinter(partName), SolveAssignResult);

            void SolveAssignResult()
            {
                var output = solutionMethods[i].Invoke(Problem, null)!;
                result.Add(partName, output);
            }
        }
        return result;
    }

    private static bool MethodPrints(MethodInfo method)
    {
        return method.HasCustomAttribute<PrintsToConsoleAttribute>()
            || method.GetCustomAttribute<PartSolutionAttribute>() is { Status: PartSolutionStatus.Interactive };
    }

    private void RunDisplayExecutionTimes(bool prints, string partName, FancyPrinting.PartLabelPrinter printer, Action runner)
    {
        bool defaultLivePrintingSetting = ExecutionTimePrinting.EnableLivePrinting;
        if (prints)
            ExecutionTimePrinting.EnableLivePrinting = false;

        if (Options.DisplayExecutionTimes)
        {
            printer(partName);
            ExecutionTimePrinting.BeginExecutionMeasuring();
        }

        runner();

        if (Options.DisplayExecutionTimes)
        {
            ExecutionTimePrinting.StopExecutionMeasuring().Wait();
        }

        ExecutionTimePrinting.EnableLivePrinting = defaultLivePrintingSetting;
    }
}

public sealed class ValidationReport
{
    private readonly Dictionary<ProblemType, ValidationProblemResult> validationResults = new();

    public IEnumerable<ValidationPartResult> AllPartValidationResults => validationResults.Values.SelectMany(problem => problem.PartResults);

    public IEnumerable<ValidationPartResult> ValidPartResults => FilterWithResult(ValidationResult.Valid);
    public IEnumerable<ValidationPartResult> InvalidPartResults => FilterWithResult(ValidationResult.Invalid);

    public bool HasInvalidParts => InvalidPartResults.Any();

    public void Add(Problem instance, ValidationProblemResult result)
    {
        validationResults.Add(ProblemsIndex.Instance.InfoForInstance(instance).ProblemType, result);
    }

    public IEnumerable<ValidationPartResult> FilterWithResult(ValidationResult result) => AllPartValidationResults.Where(problem => problem.Result == result);
}
public sealed class ValidationProblemResult
{
    public IReadOnlyCollection<ValidationPartResult> PartResults { get; }

    public ProblemType Type { get; }

    public bool HasInvalidResults => PartResults.Any(result => result.Result is ValidationResult.Invalid);

    public ValidationProblemResult(IEnumerable<ValidationPartResult> results)
    {
        PartResults = results.ToArray();
        Type = PartResults.First().Type;
    }
}
public sealed record ValidationPartResult(ProblemType Type, int Part, int TestCase, ValidationResult Result);

/// <summary>Represents a validation result.</summary>
public enum ValidationResult
{
    /// <summary>Represents a valid result.</summary>
    /// <remarks>This guarantees that the correct answer was yielded, and no errors occurred during the validation process.</remarks>
    Valid,
    /// <summary>Represents an invalid result.</summary>
    /// <remarks>This value denotes that the correct answer was not returned. It does not indicate whether the solver threw or if it only yielded an incorrect answer.</remarks>
    Invalid,

    /// <summary>Represents an aborted validation process.</summary>
    /// <remarks>Reserved for future usage.</remarks>
    Aborted,
}

public sealed class PartSolutionOutputDictionary : IEnumerable<PartSolutionOutputEntry>
{
    private readonly Dictionary<string, object> dictionary = new();

    public object? Part1Output { get; private set; }
    public object? Part2Output { get; private set; }

    public PartSolutionOutputDictionary() { }
    public PartSolutionOutputDictionary(IEnumerable<PartSolutionOutputEntry> entries)
    {
        AddRange(entries);
    }

    public void AddRange(IEnumerable<PartSolutionOutputEntry> entries)
    {
        foreach (var entry in entries)
            Add(entry);
    }
    public void Add(PartSolutionOutputEntry entry)
    {
        Add(entry.PartName, entry.Output);
    }
    public void Add(string partName, object output)
    {
        dictionary.Add(partName, output);
        AssignOfficialPartResult(partName, output);
    }
    private void AssignOfficialPartResult(string partName, object output)
    {
        switch (partName)
        {
            case "Part 1":
                Part1Output = output;
                break;

            case "Part 2":
                Part2Output = output;
                break;
        }
    }

    public object? GetPartOutput(int part)
    {
        return part switch
        {
            1 => Part1Output,
            2 => Part2Output,
            _ => null,
        };
    }

    public IEnumerator<PartSolutionOutputEntry> GetEnumerator()
    {
        return dictionary.Select(PartSolutionOutputEntry.FromKeyValuePair).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public record struct PartSolutionOutputEntry(string PartName, object Output)
{
    public static PartSolutionOutputEntry FromKeyValuePair(KeyValuePair<string, object> kvp) => new(kvp.Key, kvp.Value);
}
