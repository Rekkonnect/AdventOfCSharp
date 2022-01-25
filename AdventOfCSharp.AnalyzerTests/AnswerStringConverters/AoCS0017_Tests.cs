using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.AnswerStringConverters;

[TestClass]
public sealed class AoCS0017_Tests : AnswerStringConverterAnalyzerTests
{
    [TestMethod]
    public void ObjectAnswerStringConverter()
    {
        var testCode =
@"
namespace AoC.Converters;

public class ObjectAnswerStringConverter : ↓AnswerStringConverter<object>
{
    public override string Convert(object value)
    {
        return value.ToString();
    }
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [TestMethod]
    public void Irrelevant()
    {
        var testCode =
@"
namespace AoC.Converters;

public interface IIrrelevant<T> { }

public class Irrelevant : IIrrelevant<object>
{
}
";

        ValidateCodeWithUsings(testCode);
    }
}
