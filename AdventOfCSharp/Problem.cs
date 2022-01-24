using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCSharp;

public abstract partial class Problem
{
    private bool stateLoaded;

    public readonly InputProvider Input;

    public int Year => GetType().Namespace![^4..].ParseInt32();
    public int Day => GetType().Name["Day".Length..].ParseInt32();

    protected Problem()
    {
        Input = new(this);
    }

    public void ForceLoadState() => LoadState();

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
}

[Flags]
public enum EnabledDownloadableContentKinds
{
    None = 0,
    Input = 1 << 0,
    Output = 1 << 1,
    Both = Input | Output,
}

public class ContentDownloadingOptions
{
    public EnabledDownloadableContentKinds ContentKinds { get; set; } = EnabledDownloadableContentKinds.Both;

    public bool EnabledDownloadingInput => ContentKinds.HasFlag(EnabledDownloadableContentKinds.Input);
    public bool EnabledDownloadingOutput => ContentKinds.HasFlag(EnabledDownloadableContentKinds.Output);
}

// Content section
public abstract partial class Problem
{
    // Consider splitting into:
    // - ContentProvider
    //   - InputProvider
    //   - OutputProvider
    // At least that would semantically make more sense
    public sealed class InputProvider
    {
        private int currentTestCase;
        private string? cachedContents;

        public Problem ProblemInstance { get; }

        public int CurrentTestCase
        {
            get => currentTestCase;
            set
            {
                if (currentTestCase == value)
                    return;

                currentTestCase = value;
                cachedContents = null;
                ProblemInstance.ResetLoadedState();
            }
        }

        public ContentDownloadingOptions ContentDownloadingOptions { get; } = new();

        public int Year => ProblemInstance.Year;
        public int Day => ProblemInstance.Day;

        public string FileContents => cachedContents ??= GetInputFileContents(CurrentTestCase);
        public string NormalizedFileContents => FileContents.NormalizeLineEndings();
        public string[] UntrimmedFileLines => FileContents.GetLines();
        public string[] FileLines => FileContents.Trim().GetLines();
        public int[] FileNumbersInt32 => ParsedFileLines(int.Parse);
        public long[] FileNumbersInt64 => ParsedFileLines(long.Parse);

        public string BaseInputDirectory => BaseProblemFileDirectory(ProblemContentKind.Input);
        public int TestCaseFiles => TestCaseIDs.Count();
        public IEnumerable<int> TestCaseIDs
        {
            get
            {
                var testCaseFilePathPattern = new Regex(@"(?'day'\d*)T(?'id'\d*)\.(?'extension'.*)$");
                var validFiles = Directory.GetFiles(BaseInputDirectory).Select(f => Path.GetFileName(f)).Where(f => f.StartsWith($"{Day}T"));
                return validFiles.Select(file => testCaseFilePathPattern.Match(file).Groups["id"].Value.ParseInt32());
            }
        }

        public InputProvider(Problem instance)
        {
            ProblemInstance = instance;
        }

        public T[] ParsedFileLines<T>(Parser<T> parser) => ParsedFileLinesEnumerable(parser).ToArray();
        public T[] ParsedFileLines<T>(Parser<T> parser, int skipFirst, int skipLast) => ParsedFileLinesEnumerable(parser, skipFirst, skipLast).ToArray();
        public IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser) => ParsedFileLinesEnumerable(parser, 0, 0);
        public IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser, int skipFirst, int skipLast) => FileLines.Skip(skipFirst).SkipLast(skipLast).Select(new Func<string, T>(parser));

        private string BaseProblemFileDirectory(ProblemContentKind kind) => $@"{ProblemFiles.GetBaseDirectory()}\{kind}s\{Year}";

        private string GetInputFileLocation(int testCase) => GetFileLocation(ProblemContentKind.Input, testCase);
        private string GetOutputFileLocation(int testCase) => GetFileLocation(ProblemContentKind.Output, testCase);
        private string GetFileLocation(ProblemContentKind contentKind, int testCase) => $@"{BaseProblemFileDirectory(contentKind)}\{Day}{GetTestInputFileSuffix(testCase)}.txt";

        private static string? GetTestInputFileSuffix(int testCase) => testCase > 0 ? $"T{testCase}" : null;

        private string GetInputFileContents(int testCase)
        {
            return GetInputFileContents(testCase, ContentDownloadingOptions.EnabledDownloadingInput);
        }
        private string GetInputFileContents(int testCase, bool performDownload)
        {
            return GetProblemFileContents<string>(ProblemContentKind.Input, testCase, performDownload);
        }

        public ProblemOutput GetOutputFileContents(int testCase)
        {
            return GetOutputFileContents(testCase, ContentDownloadingOptions.EnabledDownloadingOutput);
        }
        public ProblemOutput GetOutputFileContents(int testCase, bool performDownload)
        {
            return GetProblemFileContents<ProblemOutput>(ProblemContentKind.Output, testCase, performDownload);
        }

        private TContent GetProblemFileContents<TContent>(ProblemContentKind contentKind, int testCase, bool performDownload)
        {
            var getter = BaseProblemContentGetter.GetInstance(contentKind) as BaseProblemContentGetter<TContent>;
            return getter!.GetProblemContents(this, testCase, performDownload);
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
            protected abstract ProblemContentKind ContentKind { get; }

            public static BaseProblemContentGetter GetInstance(ProblemContentKind contentKind)
            {
                return contentKind switch
                {
                    ProblemContentKind.Input => ProblemInputGetter.Instance,
                    ProblemContentKind.Output => ProblemOutputGetter.Instance,
                };
            }
        }
        private abstract class BaseProblemContentGetter<TContent> : BaseProblemContentGetter
        {
            protected abstract ContentDownloader<TContent> GetContentDownloader(InputProvider provider);
            protected abstract Func<string, TContent> GetContentParser();

            public TContent GetProblemContents(InputProvider provider, int testCase, bool performDownload)
            {
                var fileLocation = provider.GetFileLocation(ContentKind, testCase);
                if (!File.Exists(fileLocation))
                    return DownloadContent();

                var input = File.ReadAllText(fileLocation);

                if (input.Length > 0)
                    return GetContentParser()(input);

                return DownloadContent();

                TContent DownloadContent()
                {
                    return GetContentDownloader(provider)(testCase, performDownload);
                }
            }
        }

        private sealed class ProblemInputGetter : BaseProblemContentGetter<string>
        {
            public static ProblemInputGetter Instance { get; } = new();

            protected override ProblemContentKind ContentKind => ProblemContentKind.Input;

            protected override ContentDownloader<string> GetContentDownloader(InputProvider provider) => provider.DownloadInputIfMainCase;
            protected override Func<string, string> GetContentParser() => Selectors.SelfObjectReturner;
        }
        private sealed class ProblemOutputGetter : BaseProblemContentGetter<ProblemOutput>
        {
            public static ProblemOutputGetter Instance { get; } = new();

            protected override ProblemContentKind ContentKind => ProblemContentKind.Output;

            protected override ContentDownloader<ProblemOutput> GetContentDownloader(InputProvider provider) => provider.DownloadOutputIfMainCase;
            protected override Func<string, ProblemOutput> GetContentParser() => ProblemOutput.Parse;
        }

        private enum ProblemContentKind
        {
            Input,
            Output,
        }

        private delegate TContent ContentDownloader<TContent>(int testCase, bool performDownload);
    }

    public int CurrentTestCase
    {
        get => Input.CurrentTestCase;
        set => Input.CurrentTestCase = value;
    }

    // Will probably become legacy properties; consider exposing the provider and retrieve input from there
    protected string FileContents => Input.FileContents;
    protected string NormalizedFileContents => Input.NormalizedFileContents;
    protected string[] UntrimmedFileLines => Input.UntrimmedFileLines;
    protected string[] FileLines => Input.FileLines;
    protected int[] FileNumbersInt32 => Input.FileNumbersInt32;
    protected long[] FileNumbersInt64 => Input.FileNumbersInt64;

    protected string BaseInputDirectory => Input.BaseInputDirectory;

    protected T[] ParsedFileLines<T>(Parser<T> parser) => ParsedFileLinesEnumerable(parser).ToArray();
    protected T[] ParsedFileLines<T>(Parser<T> parser, int skipFirst, int skipLast) => ParsedFileLinesEnumerable(parser, skipFirst, skipLast).ToArray();
    protected IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser) => ParsedFileLinesEnumerable(parser, 0, 0);
    protected IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser, int skipFirst, int skipLast) => FileLines.Skip(skipFirst).SkipLast(skipLast).Select(new Func<string, T>(parser));
}

public abstract class Problem<T1, T2> : Problem
    where T1 : notnull
    where T2 : notnull
{
    [PartSolver("Part 1")]
    public abstract T1 SolvePart1();
    [PartSolver("Part 2")]
    public abstract T2 SolvePart2();
}

public abstract class Problem<T> : Problem<T, T>
    where T : notnull
{
}
