using AdventOfCSharp.Utilities;
using System.Globalization;

namespace AdventOfCSharp.Resources;

public sealed class GridFont
{
    private readonly GlyphStorage storage = new();

    public string? Name { get; init; }

    public int Height { get; }

    public GridFont(IEnumerable<GridGlyph> glyphs, string? name = null)
    {
        Name = name;

        var glyphArray = glyphs.ToArray();
        AddGlyphs(glyphArray);

        var first = glyphArray.First();
        Height = first.Height;
    }

    public void AddGlyphs(IEnumerable<GridGlyph> glyphs)
    {
        foreach (var glyph in glyphs)
            AddGlyph(glyph);
    }
    public void AddGlyph(GridGlyph glyph)
    {
        storage.RegisterGlyph(glyph);
    }

    public char MatchCharacter(string[] glyphStringLines, int startingColumn, out int nextGlyphStartingColumn)
    {
        foreach (var glyph in storage.Glyphs)
        {
            if (glyph.Matches(glyphStringLines, startingColumn, out nextGlyphStartingColumn))
                return glyph.GlyphChar;
        }

        // Avoid deadlocks in the case of a missing glyph
        nextGlyphStartingColumn = startingColumn + 1;
        return default;
    }

    public bool CanMatch(string[] grid)
    {
        return Height == grid.Length;
    }

    public static GridFont ParseFileContents(string fileContents) => ParseFileContents(fileContents, null);
    public static GridFont ParseFileContents(string fileContents, string? name)
    {
        var glyphs = fileContents.NormalizeLineEndings().Split("\n\n");
        var parsed = glyphs.Select(GridGlyph.ParseRawEntry);
        return new(parsed, name);
    }

    private sealed class GlyphStorage
    {
        private readonly UppercaseLatinGlyphTable uppercases = new();
        private readonly LowercaseLatinGlyphTable lowercases = new();
        private readonly DigitGlyphTable digits = new();

        public IEnumerable<GridGlyph> Glyphs => uppercases.ConcatMultiple(lowercases, uppercases);

        private BaseGlyphTable? TableForCharacter(char c)
        {
            return c.GetUnicodeCategory() switch
            {
                UnicodeCategory.LowercaseLetter => lowercases,
                UnicodeCategory.UppercaseLetter => uppercases,
                UnicodeCategory.DecimalDigitNumber => digits,
                _ => null,
            };
        }

        public void RegisterGlyph(GridGlyph glyph) => this[glyph.GlyphChar] = glyph;

        public GridGlyph? this[char c]
        {
            get => TableForCharacter(c)?.ValueOrDefault(c);
            set => TableForCharacter(c)?.SetIfValidIndex(c, value);
        }
    }

    private sealed class UppercaseLatinGlyphTable : BaseGlyphTable
    {
        public UppercaseLatinGlyphTable()
            : base('A', 'Z') { }
    }
    private sealed class LowercaseLatinGlyphTable : BaseGlyphTable
    {
        public LowercaseLatinGlyphTable()
            : base('a', 'z') { }
    }
    private sealed class DigitGlyphTable : BaseGlyphTable
    {
        public DigitGlyphTable()
            : base('0', '9') { }
    }

    private abstract class BaseGlyphTable : LookupTable<GridGlyph>
    {
        protected BaseGlyphTable(char startingCharacter, char endingCharacter)
            : base(startingCharacter, endingCharacter) { }
    }
}
