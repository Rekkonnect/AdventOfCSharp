using AdventOfCSharp.Extensions;
using System.Globalization;
using System.Resources;

namespace AdventOfCSharp.Resources;

public sealed class GridFontStore
{
    public static readonly GridFontStore Default = new(GlyphResources.ResourceManager, CultureInfo.CurrentCulture);

    public IReadOnlyCollection<GridFont> Fonts { get; }
    public IDictionary<string, GridFont> NamedFonts { get; }

    public GridFont? Infrauth => NamedFonts[nameof(Infrauth)];
    public GridFont? Stargazer => NamedFonts[nameof(Stargazer)];

    public GridFontStore(ResourceManager resourceManager, CultureInfo culture)
    {
        var resourceSet = resourceManager.GetResourceSet(culture, true, true)!;
        Fonts = resourceSet.GetStringEntries().Select(ParseEntry).ToArray();
        NamedFonts = Fonts.ToDictionaryWithNotNullKeys(font => font.Name!);

        static GridFont ParseEntry(KeyValuePair<string, string> entry)
        {
            return GridFont.ParseFileContents(entry.Value, entry.Key);
        }
    }
}
