using AdventOfCSharp.Resources;
using AdventOfCSharp.Utilities;

namespace AdventOfCSharp;

public class GlyphGridAnswerStringConverter : AnswerStringConverter<IGlyphGrid>
{
    public override string Convert(IGlyphGrid value)
    {
        var drawn = value.ToString()!.Trim();
        var drawnLines = drawn.GetLines();
        var normalized = Normalize(drawnLines);
        return ParseGlyphString(normalized) ?? drawn;
    }

    private static string? ParseGlyphString(string[] glyphStringLines)
    {
        var matchingFont = GetMatchingFont(glyphStringLines);

        if (matchingFont is null)
            return null;

        int currentColumn = 0;
        var chars = new List<char>();

        while (true)
        {
            currentColumn = NextNonEmptyColumn(glyphStringLines, currentColumn);
            if (currentColumn >= glyphStringLines.First().Length)
                break;

            chars.Add(matchingFont.MatchCharacter(glyphStringLines, currentColumn, out currentColumn));
        }
        return new string(chars.ToArray());
    }

    private static int NextNonEmptyColumn(string[] stringLines, int startingColumn)
    {
        int currentColumn = startingColumn;
        while (currentColumn < stringLines.First().Length)
        {
            if (!IsEmptyColumn(stringLines, currentColumn))
                break;

            currentColumn++;
        }
        return currentColumn;
    }
    private static bool IsEmptyColumn(string[] stringLines, int column)
    {
        for (int i = 0; i < stringLines.Length; i++)
            if (stringLines[i][column] is not '.')
                return false;
        return true;
    }

    private static GridFont? GetMatchingFont(string[] glyphStringLines)
    {
        // Return the first matching font, because there will most likely not be any other
        return GridFontStore.Default.Fonts.FirstOrDefault(f => f.CanMatch(glyphStringLines));
    }

    private static string[] Normalize(string[] drawn)
    {
        var charFrequency = new NextValueCounterDictionary<char>(drawn.SelectMany(Selectors.SelfObjectReturner));
        if (charFrequency.Count != 2)
            throw new ArgumentException("The given glyph grid does must consist of exactly 2 different characters.");

        var frequencyList = charFrequency.ToList();
        frequencyList.Sort((less, greater) => greater.Value.CompareTo(less.Value));

        IEnumerable<string> replacedDrawn = drawn;
        ReplaceDrawn(0, '.');
        ReplaceDrawn(1, '#');
        return replacedDrawn.ToArray();

        void ReplaceDrawn(int frequencyIndex, char replacement)
        {
            replacedDrawn = drawn.Select(line => line.Replace(frequencyList[frequencyIndex].Key, replacement));
        }
    }
}
