using AdventOfCSharp.Extensions;
using System.IO;
using System.Text.RegularExpressions;

namespace AdventOfCSharp;

public abstract partial class Problem
{
    public bool StateLoaded => Input.StateLoaded;

    public readonly InputProvider Input;
    public readonly OutputProvider Output;

    public int Year => GetType().Namespace![^4..].ParseInt32();
    public int Day => GetType().Name["Day".Length..].ParseInt32();

    protected Problem()
    {
        Input = new(this);
        Output = new(this);
    }

    public void ForceLoadState() => LoadState();

    protected virtual void LoadState() { }
    protected virtual void ResetState() { }

    public void EnsureLoadedState()
    {
        Input.HandleStateLoading(true, LoadState);
    }
    public void ResetLoadedState()
    {
        Input.HandleStateLoading(false, ResetState);
    }

    /// <summary>Ensures that the input is downloaded.</summary>
    /// <remarks>Downloading refers to either loading the input from the device's storage, or downloading it from the server and storing it locally.</remarks>
    public void EnsureDownloadedInput()
    {
        Input.TriggerFileContentCache();
    }

    public static T CreateNewLoadedState<T>()
        where T : Problem, new()
    {
        var problem = new T();
        problem.EnsureLoadedState();
        return problem;
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
    public abstract class ContentProvider
    {
        protected static readonly Regex TestCaseFilePathPattern = new(@"(?'day'\d*)T(?'id'\d*)\.(?'extension'.*)$");

        protected int CurrentTestCaseField;

        internal int CurrentTestCase
        {
            get => CurrentTestCaseField;
            set
            {
                if (CurrentTestCaseField == value)
                    return;

                CurrentTestCaseField = value;
                ResetCachedContents();
                ProblemInstance.ResetLoadedState();
            }
        }

        public Problem ProblemInstance { get; }

        public int Year => ProblemInstance.Year;
        public int Day => ProblemInstance.Day;

        public ContentDownloadingOptions ContentDownloadingOptions { get; } = new();

        protected ContentProvider(Problem instance)
        {
            ProblemInstance = instance;
        }

        protected abstract void ResetCachedContents();

        internal string BaseProblemFileDirectory(ProblemContentKind kind) => $@"{ProblemFiles.GetBaseDirectory()}\{kind}s\{Year}";

        internal string GetFileLocation(ProblemContentKind contentKind, int testCase) => $@"{BaseProblemFileDirectory(contentKind)}\{Day}{GetTestInputFileSuffix(testCase)}.txt";

        private static string? GetTestInputFileSuffix(int testCase) => testCase > 0 ? $"T{testCase}" : null;

        internal TContent GetProblemFileContents<TContent>(ProblemContentKind contentKind, int testCase, bool performDownload)
        {
            var getter = BaseProblemContentGetter.GetInstance(contentKind) as BaseProblemContentGetter<TContent>;
            return getter!.GetProblemContents(this, testCase, performDownload);
        }

        protected static TContent DownloadContentIfMainCase<TContent>(int testCase, bool performDownload, Func<TContent> contentDownloader, TContent empty)
        {
            if (testCase is 0 && performDownload)
                return contentDownloader();

            return empty;
        }

        internal abstract class BaseProblemContentGetter
        {
            protected abstract ProblemContentKind ContentKind { get; }

            public static BaseProblemContentGetter GetInstance(ProblemContentKind contentKind)
            {
                return contentKind switch
                {
                    ProblemContentKind.Input => InputProvider.ProblemInputGetter.Instance,
                    ProblemContentKind.Output => OutputProvider.ProblemOutputGetter.Instance,
                };
            }
        }
        internal abstract class BaseProblemContentGetter<TContent> : BaseProblemContentGetter
        {
            protected abstract ContentDownloader<TContent> GetContentDownloader(ContentProvider provider);
            protected abstract Func<string, TContent> GetContentParser();

            public TContent GetProblemContents(ContentProvider provider, int testCase, bool performDownload)
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

        internal enum ProblemContentKind
        {
            Input,
            Output,
        }

        internal delegate TContent ContentDownloader<TContent>(int testCase, bool performDownload);
    }

    public sealed class OutputProvider : ContentProvider
    {
        public OutputProvider(Problem instance)
            : base(instance) { }

        protected override void ResetCachedContents() { }

        private string GetOutputFileLocation(int testCase) => GetFileLocation(ProblemContentKind.Output, testCase);

        public ProblemOutput GetOutputFileContents(int testCase)
        {
            return GetOutputFileContents(testCase, ContentDownloadingOptions.EnabledDownloadingOutput);
        }
        public ProblemOutput GetOutputFileContents(int testCase, bool performDownload)
        {
            return GetProblemFileContents<ProblemOutput>(ProblemContentKind.Output, testCase, performDownload);
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

        internal sealed class ProblemOutputGetter : BaseProblemContentGetter<ProblemOutput>
        {
            public static ProblemOutputGetter Instance { get; } = new();

            protected override ProblemContentKind ContentKind => ProblemContentKind.Output;

            protected override ContentDownloader<ProblemOutput> GetContentDownloader(ContentProvider provider) => (provider as OutputProvider)!.DownloadOutputIfMainCase;
            protected override Func<string, ProblemOutput> GetContentParser() => ProblemOutput.Parse;
        }
    }

    public sealed class InputProvider : ContentProvider
    {
        private string? cachedContents;

        public bool StateLoaded { get; private set; }

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
                var baseInputDirectoryInfo = new DirectoryInfo(BaseInputDirectory);
                if (!baseInputDirectoryInfo.Exists)
                    return Enumerable.Empty<int>();

                var allFiles = baseInputDirectoryInfo.GetFiles();
                var validFiles = allFiles.Where(f => f.Name.StartsWith($"{Day}T"));
                return validFiles.Select(file => TestCaseFilePathPattern.Match(file.Name).Groups["id"].Value.ParseInt32());
            }
        }

        public InputProvider(Problem instance)
            : base(instance) { }

        internal void HandleStateLoading(bool targetStateLoadedStatus, Action stateHandler)
        {
            if (StateLoaded == targetStateLoadedStatus)
                return;

            stateHandler();
            StateLoaded = targetStateLoadedStatus;
        }

        protected override void ResetCachedContents()
        {
            cachedContents = null;
        }

        public void SetCustomGeneratedContents(string fileContents)
        {
            cachedContents = fileContents;
            CurrentTestCaseField = -1;
        }

        public void TriggerFileContentCache()
        {
            _ = FileContents;
        }

        public T[] ParsedFileLines<T>(Parser<T> parser) => ParsedFileLinesEnumerable(parser).ToArray();
        public T[] ParsedFileLines<T>(Parser<T> parser, int skipFirst, int skipLast) => ParsedFileLinesEnumerable(parser, skipFirst, skipLast).ToArray();
        public IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser) => ParsedFileLinesEnumerable(parser, 0, 0);
        public IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser, int skipFirst, int skipLast) => FileLines.Skip(skipFirst).SkipLast(skipLast).Select(new Func<string, T>(parser));

        private string GetInputFileLocation(int testCase) => GetFileLocation(ProblemContentKind.Input, testCase);

        private string GetInputFileContents(int testCase)
        {
            return GetInputFileContents(testCase, ContentDownloadingOptions.EnabledDownloadingInput);
        }
        private string GetInputFileContents(int testCase, bool performDownload)
        {
            var contents = GetProblemFileContents<string>(ProblemContentKind.Input, testCase, performDownload);
            if (contents.IsNullOrEmpty())
                throw new IOException("The requested input for the specified problem was not found locally, or could not be downloaded. Ensure that downloading is enabled and that the secrets are valid.");

            return contents;
        }

        private string DownloadInputIfMainCase(int testCase, bool performDownload)
        {
            return DownloadContentIfMainCase(testCase, performDownload, DownloadSaveInput, "");
        }
        private string DownloadSaveInput()
        {
            var input = WebsiteScraping.DownloadInput(Year, Day);
            if (input is not null)
            {
                FileHelpers.WriteAllTextEnsuringDirectory(GetInputFileLocation(0), input);
            }
            // Silence warning because we throw the exception at another point when the input is returned as null
            return input!;
        }

        internal sealed class ProblemInputGetter : BaseProblemContentGetter<string>
        {
            public static ProblemInputGetter Instance { get; } = new();

            protected override ProblemContentKind ContentKind => ProblemContentKind.Input;

            protected override ContentDownloader<string> GetContentDownloader(ContentProvider provider) => (provider as InputProvider)!.DownloadInputIfMainCase;
            protected override Func<string, string> GetContentParser() => Selectors.SelfObjectReturner;
        }
    }

    public int CurrentTestCase
    {
        get => Input.CurrentTestCase;
        set
        {
            Input.CurrentTestCase = value;
            Output.CurrentTestCase = value;
        }
    }

    // These properties are here for
    // - compatibility reasons
    // - providing fluency when working with input
    protected string FileContents => Input.FileContents;
    protected string NormalizedFileContents => Input.NormalizedFileContents;
    protected string[] UntrimmedFileLines => Input.UntrimmedFileLines;
    protected string[] FileLines => Input.FileLines;
    protected int[] FileNumbersInt32 => Input.FileNumbersInt32;
    protected long[] FileNumbersInt64 => Input.FileNumbersInt64;

    protected string BaseInputDirectory => Input.BaseInputDirectory;

    protected T[] ParsedFileLines<T>(Parser<T> parser)
    {
        return ParsedFileLinesEnumerable(parser).ToArray();
    }
    protected T[] ParsedFileLines<T>(Parser<T> parser, int skipFirst, int skipLast)
    {
        return ParsedFileLinesEnumerable(parser, skipFirst, skipLast).ToArray();
    }
    protected IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser)
    {
        return ParsedFileLinesEnumerable(parser, 0, 0);
    }
    protected IEnumerable<T> ParsedFileLinesEnumerable<T>(Parser<T> parser, int skipFirst, int skipLast)
    {
        return FileLines.Skip(skipFirst).SkipLast(skipLast).Select(new Func<string, T>(parser));
    }
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

// Warning on our own framework, that's how powerful analyzers are
#pragma warning disable AoCS0003 // Prefer using Problem<T> for problems whose both parts have the same return type
public abstract class Problem<T> : Problem<T, T>
#pragma warning restore AoCS0003 // Prefer using Problem<T> for problems whose both parts have the same return type
    where T : notnull
{
}
