using System.IO;

namespace AdventOfCSharp;

/*
 * This class should be split into:
 * - Input loading and providing
 * - Running
 * - Solving
 */
public abstract class Problem
{
    private bool stateLoaded;
    private int currentTestCase;

    public int CurrentTestCase
    {
        get => currentTestCase;
        set
        {
            if (currentTestCase == value)
                return;

            currentTestCase = value;
            ResetLoadedState();
        }
    }

    protected string FileContents => GetInputFileContents(CurrentTestCase);
    protected string NormalizedFileContents => GetInputFileContents(CurrentTestCase).NormalizeLineEndings();
    protected string[] UntrimmedFileLines => FileContents.GetLines();
    protected string[] FileLines => FileContents.Trim().GetLines();
    protected int[] FileNumbersInt32 => ParsedFileLines(int.Parse);
    protected long[] FileNumbersInt64 => ParsedFileLines(long.Parse);

    public int Year => GetType().Namespace![^4..].ParseInt32();
    public int Day => GetType().Name["Day".Length..].ParseInt32();

    protected string BaseInputDirectory => BaseProblemFileDirectory(ProblemFileKind.Input);
    public int TestCaseFiles => Directory.GetFiles(BaseInputDirectory).Count(f => Path.GetFileName(f).StartsWith($"{Day}T"));

    protected T[] ParsedFileLines<T>(Parser<T> parser) => ParsedFileLinesEnumerable(parser).ToArray();
    protected T[] ParsedFileLines<T>(Parser<T> parser, int skipFirst, int skipLast) => ParsedFileLinesEnumerable(parser, skipFirst, skipLast).ToArray();
    protected IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser) => ParsedFileLinesEnumerable(parser, 0, 0);
    protected IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser, int skipFirst, int skipLast) => FileLines.Skip(skipFirst).SkipLast(skipLast).Select(new Func<string, T>(parser));

    protected virtual void LoadState() { }
    protected virtual void ResetState() { }

    public void EnsureLoadedState()
    {
        HandleStateLoading(true, LoadState);
    }
    public void ResetLoadedState()
    {
        HandleStateLoading(false, ResetState);
    }

    private void HandleStateLoading(bool targetStateLoadedStatus, Action stateHandler)
    {
        if (stateLoaded == targetStateLoadedStatus)
            return;
        stateHandler();
        stateLoaded = targetStateLoadedStatus;
    }

    private string BaseProblemFileDirectory(ProblemFileKind kind) => $@"{ProblemFiles.GetBaseDirectory()}\{kind}s\{Year}";

    private string GetInputFileLocation(int testCase) => GetFileLocation(ProblemFileKind.Input, testCase);
    private string GetOutputFileLocation(int testCase) => GetFileLocation(ProblemFileKind.Output, testCase);
    private string GetFileLocation(ProblemFileKind fileKind, int testCase) => $@"{BaseProblemFileDirectory(fileKind)}\{Day}{GetTestInputFileSuffix(testCase)}.txt";

    private static string? GetTestInputFileSuffix(int testCase) => testCase > 0 ? $"T{testCase}" : null;

    // TODO: Introduce a property in the input handler that toggles performing the download if unavailable input
    private string GetInputFileContents(int testCase, bool performDownload = true)
    {
        return GetProblemFileContents<string>(ProblemFileKind.Input, testCase, performDownload);
    }

    public ProblemOutput GetOutputFileContents(int testCase, bool performDownload = true)
    {
        return GetProblemFileContents<ProblemOutput>(ProblemFileKind.Output, testCase, performDownload);
    }

    private TContent GetProblemFileContents<TContent>(ProblemFileKind fileKind, int testCase, bool performDownload)
    {
        var getter = BaseProblemContentGetter.GetInstance(fileKind) as BaseProblemContentGetter<TContent>;
        return getter!.GetProblemFileContents(this, testCase, performDownload);
    }

    private string DownloadInputIfMainCase(int testCase, bool performDownload)
    {
        return DownloadContentIfMainCase(testCase, performDownload, DownloadSaveInput, "");
    }
    private string DownloadSaveInput()
    {
        var input = WebsiteScraping.DownloadInput(Year, Day);
        FileHelpers.WriteAllTextEnsuringDirectory(GetInputFileLocation(0), input);
        return input;
    }

    private ProblemOutput DownloadOutputIfMainCase(int testCase, bool performDownload)
    {
        return DownloadContentIfMainCase(testCase, performDownload, DownloadSaveCorrectOutput, ProblemOutput.Empty);
    }
    private ProblemOutput DownloadSaveCorrectOutput()
    {
        var output = WebsiteScraping.DownloadAnsweredCorrectOutputs(Year, Day);
        FileHelpers.WriteAllTextEnsuringDirectory(GetOutputFileLocation(0), output.GetFileString());
        return output;
    }

    private static TContent DownloadContentIfMainCase<TContent>(int testCase, bool performDownload, Func<TContent> contentDownloader, TContent empty)
    {
        if (testCase is 0 && performDownload)
            return contentDownloader();

        return empty;
    }

    private abstract class BaseProblemContentGetter
    {
        protected abstract ProblemFileKind ContentKind { get; }

        public static BaseProblemContentGetter GetInstance(ProblemFileKind contentKind)
        {
            return contentKind switch
            {
                ProblemFileKind.Input => ProblemInputGetter.Instance,
                ProblemFileKind.Output => ProblemOutputGetter.Instance,
            };
        }
    }
    private abstract class BaseProblemContentGetter<TContent> : BaseProblemContentGetter
    {
        protected abstract ContentDownloader<TContent> GetContentDownloader(Problem problem);
        protected abstract Func<string, TContent> GetContentParser(Problem problem);

        public TContent GetProblemFileContents(Problem problem, int testCase, bool performDownload)
        {
            var fileLocation = problem.GetFileLocation(ContentKind, testCase);
            if (!File.Exists(fileLocation))
                return DownloadContent();

            var input = File.ReadAllText(fileLocation);

            if (input.Length > 0)
                return GetContentParser(problem)(input);

            return DownloadContent();

            TContent DownloadContent()
            {
                return GetContentDownloader(problem)(testCase, performDownload);
            }
        }
    }

    private sealed class ProblemInputGetter : BaseProblemContentGetter<string>
    {
        public static ProblemInputGetter Instance { get; } = new();

        protected override ProblemFileKind ContentKind => ProblemFileKind.Input;

        protected override ContentDownloader<string> GetContentDownloader(Problem problem) => problem.DownloadInputIfMainCase;
        protected override Func<string, string> GetContentParser(Problem problem) => Selectors.SelfObjectReturner;
    }
    private sealed class ProblemOutputGetter : BaseProblemContentGetter<ProblemOutput>
    {
        public static ProblemOutputGetter Instance { get; } = new();

        protected override ProblemFileKind ContentKind => ProblemFileKind.Output;

        protected override ContentDownloader<ProblemOutput> GetContentDownloader(Problem problem) => problem.DownloadOutputIfMainCase;
        protected override Func<string, ProblemOutput> GetContentParser(Problem problem) => ProblemOutput.Parse;
    }

    private enum ProblemFileKind
    {
        Input,
        Output,
    }

    private delegate TContent ContentDownloader<TContent>(int testCase, bool performDownload);
}

public abstract class Problem<T1, T2> : Problem
{
    public T1 RunPart1()
    {
        EnsureLoadedState();
        return SolvePart1();
    }
    public T2 RunPart2()
    {
        EnsureLoadedState();
        return SolvePart2();
    }

    public abstract T1 SolvePart1();
    public abstract T2 SolvePart2();
}

public abstract class Problem<T> : Problem<T, T> { }
