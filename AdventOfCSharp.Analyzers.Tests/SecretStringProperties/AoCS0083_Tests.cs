using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.SecretStringProperties;

[TestClass]
public sealed class AoCS0083_Tests : SecretStringPropertyAnalyzerTests
{
    [DataTestMethod]
    [DataRow("bool")]
    [DataRow("char")]
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
    public void NotStringSecretType(string type)
    {
        string testCode =
$@"
public abstract class NonNumericalSecrets : TestSecretsContainerBase
{{
    [SecretStringProperty(@""\d"", ""numerical"", SecretsType)]
    public abstract ↓{type} Numerical {{ get; }}
}}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
