using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace AdventOfCSharp.SourceGenerators.Tests;

public static class SourceFileListExtensions
{
    public static void AddRange(this SourceFileList list, IEnumerable<string> sources)
    {
        foreach (var source in sources)
            list.Add(source);
    }
    public static void AddRange(this SourceFileList list, IEnumerable<SourceText> sources)
    {
        foreach (var source in sources)
            list.Add(source);
    }
}