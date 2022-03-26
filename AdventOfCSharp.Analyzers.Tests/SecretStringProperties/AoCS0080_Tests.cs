using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.SecretStringProperties;

[TestClass]
public sealed class AoCS0080_Tests : SecretStringPropertyAnalyzerTests
{
    [TestMethod]
    public void SecretsPatternNotMatching()
    {
        const string testCode =
@"
public sealed class SomeSecrets : TestSecretsContainerBase
{
    [SecretStringProperty(@""\d\w\d"", ""_test"", SecretsType)]
    public string SomeSecrets => ↓""not matching pattern"";
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
    [TestMethod]
    public void SecretsPatternMatching()
    {
        const string testCode =
@"
public sealed class SomeSecrets : TestSecretsContainerBase
{
    [SecretStringProperty(@""\w{5}\d{2}"", ""_testRight"", SecretsType)]
    public string CorrectPattern => ""abcde12"";
}
";

        ValidateCodeWithUsings(testCode);
    }
}
