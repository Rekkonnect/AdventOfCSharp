namespace AdventOfCSharp.Resources;

public sealed class GridGlyph
{
    private readonly string[] lines;

    public char GlyphChar { get; set; }
    public int Width { get; }
    public int Height => lines.Length;

    internal GridGlyph(char glyphChar, string[] sequenceLines)
    {
        GlyphChar = glyphChar;
        lines = sequenceLines;
        Width = lines.Max(line => line.Length);
    }

    public bool Matches(string[] glyphStringLines, int startingColumn, out int nextGlyphStartingColumn)
    {
        nextGlyphStartingColumn = startingColumn;

        int currentColumn = startingColumn;
        for (int x = 0; x < Width; x++, currentColumn++)
        {
            for (int y = 0; y < Height; y++)
            {
                // Consider that the resource is sanitized:
                // - all glyphs will only consist of . and #
                // - every line has the same width

                if (glyphStringLines[y][currentColumn] != lines[y][x])
                    return false;
            }
        }

        nextGlyphStartingColumn = currentColumn;
        return true;
    }

    internal static GridGlyph ParseRawEntry(string normalizedRawEntry)
    {
        // The glyphs are strictly stored in the following form:
        /*
         * [Character]
         * [Sequence of . and # for the first line]
         * [Sequence for the second line]
         * [etc.]
         * 
         * Example:
         * 
         * 0
         * .##.
         * #..#
         * #..#
         * #..#
         * .##.
         */

        char glyphChar = normalizedRawEntry[0];
        string representation = normalizedRawEntry[(normalizedRawEntry.IndexOf('\n') + 1)..];
        return new(glyphChar, representation.GetLines(false));
    }
}
