using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCSharp.SourceGenerators.Utilities;

public sealed class GeneratedSourceMappings : SortedDictionary<string, SourceText>
{
    public GeneratedSourceMappings() { }
    public GeneratedSourceMappings(IComparer<string> comparer)
        : base(comparer) { }

    public void Add(string hintName, string source)
    {
        Add(hintName, SourceText.From(source, Encoding.UTF8));
    }
    
    public void Set(string hintName, string source)
    {
        this[hintName] = SourceText.From(source, Encoding.UTF8);
    }
}
