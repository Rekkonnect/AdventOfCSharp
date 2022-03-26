using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.AnswerStringConverters;

[TestClass]
public sealed class AoCS0016_Tests : AnswerStringConverterAnalyzerTests
{
    [TestMethod]
    public void DirectlyInheritAnswerStringConverter()
    {
        var testCode =
@"
namespace AoC.Converters;

public class RawAnswerStringConverter : ↓AnswerStringConverter
{
    public string Convert(object value)
    {
        return value.ToString();
    }
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
