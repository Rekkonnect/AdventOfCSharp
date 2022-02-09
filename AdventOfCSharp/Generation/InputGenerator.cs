using System.Text;

namespace AdventOfCSharp.Generation;

/// <summary>Provides mechanisms for generating custom input strings for a specified problem.</summary>
public abstract partial class InputGenerator
{
    /// <summary>The year of the problem.</summary>
    public abstract int Year { get; }
    /// <summary>The day of the problem.</summary>
    public abstract int Day { get; }

    /// <summary>Gets or initializes the <seealso cref="Random"/> instance that will be used when generating values.</summary>
    public Random Randomizer { get; protected init; } = Random.Shared;

    /// <summary>Determines whether the generator always generates valid inputs.</summary>
    /// <remarks>Defaults to <see langword="false"/>.</remarks>
    protected virtual bool AlwaysGeneratesValidInputs => false;

    /// <summary>Initializes a new <seealso cref="Problem"/> instance for the problem whose input to generate.</summary>
    /// <returns>The initialized <seealso cref="Problem"/> instance.</returns>
    public Problem InitializeProblemInstance()
    {
        return ProblemsIndex.Instance[Year, Day].InitializeInstance()!;
    }

    /// <summary>Generates the next input string.</summary>
    /// <returns>The next generated input string.</returns>
    public abstract string GenerateNext();

    /// <summary>Generates a valid input string.</summary>
    /// <returns>The generated input string.</returns>
    /// <remarks>If <seealso cref="AlwaysGeneratesValidInputs"/> is <see langword="true"/>, the first generated input string is returned.</remarks>
    public string GenerateNextValid()
    {
        if (AlwaysGeneratesValidInputs)
            return GenerateNext();

        while (true)
        {
            var generated = GenerateNext();

            if (IsValidInput(generated))
                return generated;
        }
    }
    /// <summary>Generates a number of valid input strings.</summary>
    /// <param name="count">The number of input strings to generate.</param>
    /// <returns>The generated input strings.</returns>
    public IEnumerable<string> GenerateNextValid(int count)
    {
        for (int i = 0; i < count; i++)
            yield return GenerateNextValid();
    }

    /// <summary>Determines whether the input string is valid.</summary>
    /// <param name="input">The input string whose validity to determine.</param>
    /// <returns>
    /// <see langword="true"/> if the <seealso cref="AlwaysGeneratesValidInputs"/> property is <see langword="true"/>,
    /// otherwise a value determining whether the input string was evaluated as valid.
    /// </returns>
    /// <remarks>This method defaults to running the solution for both official parts, demanding that no exceptions are thrown and the results are not <see langword="null"/>.</remarks>
    public virtual bool IsValidInput(string input)
    {
        if (AlwaysGeneratesValidInputs)
            return true;

        var problem = InitializeProblemInstance();
        problem.Input.SetCustomGeneratedContents(input);
        try
        {
            var runner = new ProblemRunner(problem);
            var results = runner.SolveAllOfficialParts();

            if (results.Part1Output is null)
                return false;
            if (results.Part2Output is null)
                return false;
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}

// Fluent API
public abstract partial class InputGenerator
{
    /// <summary>Generates a new sequence of integers of the specified count.</summary>
    /// <param name="count">The number of integers to include in the sequence.</param>
    /// <returns>The <seealso cref="IntegerSequenceInputGenerationInfo"/> that contains information about the sequence to generate.</returns>
    protected IntegerSequenceInputGenerationInfo Integers(int count) => new(this, count);
    /// <summary>Generates a new sequence of integers, with a count ranging in the specified inclusive count range.</summary>
    /// <param name="minCount">The minimum number (inclusive) of integers to include in the sequence.</param>
    /// <param name="maxCount">The maximum number (inclusive) of integers to include in the sequence.</param>
    /// <returns>The <seealso cref="IntegerSequenceInputGenerationInfo"/> that contains information about the sequence to generate.</returns>
    protected IntegerSequenceInputGenerationInfo Integers(int minCount, int maxCount) => new(this, minCount, maxCount);

    /// <summary>Generates a new sequence of specified input string segments.</summary>
    /// <param name="segments">The segments the input string will consist of.</param>
    /// <returns>The <seealso cref="SegmentedInputGenerationInfo"/> that contains information about the sequence of segments to generate.</returns>
    protected SegmentedInputGenerationInfo Segmented(params InputGenerationInfo[] segments) => new(this, segments);

    public abstract class InputGenerationInfo
    {
        public InputGenerator Generator { get; }

        protected InputGenerationInfo(InputGenerator generator)
        {
            Generator = generator;
        }

        /// <summary>Generates the input string.</summary>
        /// <returns>The generated input string.</returns>
        public abstract string Build();
    }
    public abstract class DelimitedInputGenerationInfo : InputGenerationInfo
    {
        /// <summary>Gets or sets the delimiter that will be applied between the generated sequence's elements.</summary>
        public string Delimiter { get; set; } = Environment.NewLine;

        protected DelimitedInputGenerationInfo(InputGenerator generator)
            : base(generator) { }

        /// <summary>Sets the delimiter to <see cref="Environment.NewLine"/>.</summary>
        /// <returns>This <seealso cref="DelimitedInputGenerationInfo"/> instance.</returns>
        public DelimitedInputGenerationInfo PerLine()
        {
            return DelimitedBy(Environment.NewLine);
        }
        /// <summary>Sets the delimiter to <see cref="Environment.NewLine"/> repeated by the specified number of times.</summary>
        /// <param name="lines">The number of lines that will be applied as a delimiter.</param>
        /// <returns>This <seealso cref="DelimitedInputGenerationInfo"/> instance.</returns>
        public DelimitedInputGenerationInfo PerLines(int lines)
        {
            // A string.Repeat extension would be widely appreciated
            var newline = Environment.NewLine;
            var multilineDelimiter = newline;
            for (int i = 1; i < lines; i++)
                multilineDelimiter += newline;

            return DelimitedBy(multilineDelimiter);
        }
        /// <summary>Sets the delimiter to the specified string.</summary>
        /// <param name="delimiter">The delimiter to set.</param>
        /// <returns>This <seealso cref="DelimitedInputGenerationInfo"/> instance.</returns>
        public DelimitedInputGenerationInfo DelimitedBy(string delimiter)
        {
            Delimiter = delimiter;
            return this;
        }
    }
    public abstract class ElementSequenceInputGenerationInfo : ElementSequenceInputGenerationInfo<object>
    {
        protected ElementSequenceInputGenerationInfo(InputGenerator generator, int count = 0)
            : base(generator, count) { }
        protected ElementSequenceInputGenerationInfo(InputGenerator generator, int minCount, int maxCount)
            : base(generator, minCount, maxCount) { }
    }
    public abstract class ElementSequenceInputGenerationInfo<T> : DelimitedInputGenerationInfo
    {
        /// <summary>Gets a random count within the specified range from <seealso cref="MinCount"/> and <seealso cref="MaxCount"/>, or sets the fixed element count of the sequence.</summary>
        /// <remarks>If <seealso cref="MinCount"/> and <seealso cref="MaxCount"/> are equal, the count is the specified fixed one.</remarks>
        public int Count
        {
            get
            {
                if (MinCount == MaxCount)
                    return MinCount;

                return Generator.Randomizer.Next(MinCount, MaxCount + 1);
            }
            set
            {
                MinCount = value;
                MaxCount = value;
            }
        }

        /// <summary>Gets or sets the minimum count (inclusive) of elements the resulting sequence will contain.</summary>
        public int MinCount { get; set; }
        /// <summary>Gets or sets the maximum count (inclusive) of elements the resulting sequence will contain.</summary>
        public int MaxCount { get; set; }

        protected ElementSequenceInputGenerationInfo(InputGenerator generator, int count = 0)
            : base(generator)
        {
            Count = count;
        }
        protected ElementSequenceInputGenerationInfo(InputGenerator generator, int minCount, int maxCount)
            : base(generator)
        {
            MinCount = minCount;
            MaxCount = maxCount;
        }

        /// <summary>Generates the next element in the sequence.</summary>
        /// <returns>The generated element to be appended to the sequence.</returns>
        public abstract T GenerateNextElement();

        /// <inheritdoc/>
        public override string Build()
        {
            var builder = new StringBuilder();

            for (int i = 0; i < Count; i++)
                builder.Append(GenerateNextElement()).Append(Delimiter);

            return builder.RemoveLast(Delimiter.Length).ToString();
        }
    }
    public sealed class SegmentedInputGenerationInfo : DelimitedInputGenerationInfo
    {
        private readonly InputGenerationInfo[] segments;

        public SegmentedInputGenerationInfo(InputGenerator generator, params InputGenerationInfo[] generationSegments)
            : base(generator)
        {
            segments = generationSegments;
        }

        public override string Build()
        {
            var builder = new StringBuilder();

            foreach (var segment in segments)
                builder.Append(segment.Build()).Append(Delimiter);
            
            return builder.RemoveLast(Delimiter.Length).ToString();
        }
    }

    public sealed class IntegerSequenceInputGenerationInfo : ElementSequenceInputGenerationInfo<int>
    {
        /// <summary>Gets or sets the minimum value (inclusive) to be contained in the sequence.</summary>
        public int Min { get; set; }
        /// <summary>Gets or sets the maximum value (inclusive) to be contained in the sequence.</summary>
        public int Max { get; set; }

        public IntegerSequenceInputGenerationInfo(InputGenerator generator, int count = 0)
            : base(generator, count) { }
        public IntegerSequenceInputGenerationInfo(InputGenerator generator, int minCount, int maxCount)
            : base(generator, minCount, maxCount) { }

        /// <summary>Sets the minimum and maximum values that will be contained in the sequence.</summary>
        /// <param name="min">The minimum value (inclusive) to be contained in the sequence.</param>
        /// <param name="max">The maximum value (inclusive) to be contained in the sequence.</param>
        /// <returns>This <seealso cref="IntegerSequenceInputGenerationInfo"/> instance.</returns>
        public IntegerSequenceInputGenerationInfo Ranging(int min, int max)
        {
            Min = min;
            Max = max;
            return this;
        }

        /// <inheritdoc/>
        public override int GenerateNextElement()
        {
            return Generator.Randomizer.Next(Min, Max + 1);
        }
    }
}

// PoC
public sealed class Year2021Day4InputGenerator : InputGenerator
{
    public override int Year => 2021;
    public override int Day => 4;

    public override string GenerateNext()
    {
        return Segmented
               (
                   Integers(60, 80).Ranging(0, 100).DelimitedBy(","),
                   new BingoBoardGeneratorInfo(this, 100).PerLines(2)
               ).PerLines(2).Build();
    }

    public sealed class BingoBoardGeneratorInfo : ElementSequenceInputGenerationInfo<BingoBoardGeneratorInfo.BingoBoard>
    {
        public BingoBoardGeneratorInfo(InputGenerator generator, int count)
            : base(generator, count) { }

        public override BingoBoard GenerateNextElement()
        {
            return null;
        }

        public class BingoBoard
        {

        }
    }
}
