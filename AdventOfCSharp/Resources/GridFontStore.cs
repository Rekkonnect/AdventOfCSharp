using AdventOfCSharp.Extensions;
using System.Globalization;
using System.Resources;

namespace AdventOfCSharp.Resources;

public sealed class GridFontStore
{
    public static readonly GridFontStore Default = new(GlyphResources.ResourceManager, CultureInfo.CurrentCulture);

    public IReadOnlyCollection<GridFont> Fonts { get; }

    public GridFontStore(ResourceManager resourceManager, CultureInfo culture)
    {
        var resourceSet = resourceManager.GetResourceSet(culture, true, true)!;
        Fonts = resourceSet.GetStrings().Select(GridFont.ParseFileContents).ToArray();
    }
}
