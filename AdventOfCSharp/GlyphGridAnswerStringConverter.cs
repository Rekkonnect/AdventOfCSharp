using AdventOfCSharp.Utilities;

namespace AdventOfCSharp;

public class GlyphGridAnswerStringConverter : ObjectAnswerStringConverter
{
    // So far the baseline is written, and it will soon be available
    public override string Convert(object value)
    {
        // For the time being use this, until the converter is implemented
        return CommonAnswerStringConverter.Instance.Convert(value);

        var drawn = value.ToString();
        var drawnLines = drawn.GetLines();
        var normalized = Normalize(drawnLines);

        // TODO: Implement this
        string parsed = null;

        return parsed;
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
