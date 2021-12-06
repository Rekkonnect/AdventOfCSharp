#nullable enable

using AdventOfCSharp.Extensions;

namespace AdventOfCSharp;

public record ProblemOutput(string? Part1, string? Part2)
{
    public static ProblemOutput Empty { get; } = new(null, null);

    public string GetFileString()
    {
        if (Part1 is null)
            return "";

        if (Part2 is null)
            return Part1;

        return $"{Part1}\n{Part2}";
    }

    public string? ForPart(int part) => part switch
    {
        1 => Part1,
        2 => Part2,
    };

    public static ProblemOutput Parse(string fileString)
    {
        return Parse(fileString.GetLines());
    }
    public static ProblemOutput Parse(string[] lines)
    {
        string? part1 = null;
        string? part2 = null;

        SetIfAvailable(ref part1, lines, 0);
        SetIfAvailable(ref part2, lines, 1);

        return new(part1, part2);

        static void SetIfAvailable(ref string? result, string[] array, int index)
        {
            if (array.Length > index)
                result = array[index].NullIfEmpty();
        }
    }
}
