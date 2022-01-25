using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.AnswerStringConverters;

[TestClass]
public sealed class AoCS0018_Tests : AnswerStringConverterAnalyzerTests
{
    [DataTestMethod]
    [DataRow("byte")]
    [DataRow("short")]
    [DataRow("int")]
    [DataRow("long")]
    [DataRow("sbyte")]
    [DataRow("ushort")]
    [DataRow("uint")]
    [DataRow("ulong")]
    [DataRow("float")]
    [DataRow("double")]
    [DataRow("decimal")]
    [DataRow("string")]
    public void FrameworkHandledType(string typeKeyword)
    {
        var testCode =
$@"
namespace AoC.Converters;

public class HandledTypeAnswerStringConverter : ↓AnswerStringConverter<{typeKeyword}>
{{
    public override string Convert({typeKeyword} value)
    {{
        return value.ToString();
    }}
}}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [TestMethod]
    public void CustomData()
    {
        var testCode =
@"
namespace AoC.Converters;

public interface ICustomData { }

public class CustomData : AnswerStringConverter<ICustomData>
{
    public override string Convert(ICustomData custom)
    {
        return custom.ToString();
    }
}
";

        ValidateCodeWithUsings(testCode);
    }
}
