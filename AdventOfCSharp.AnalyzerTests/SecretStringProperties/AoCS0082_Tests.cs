using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.SecretStringProperties;

[TestClass]
public sealed class AoCS0082_Tests : SecretStringPropertyAnalyzerTests
{
    [TestMethod]
    public void NotConstantFieldForSecretType()
    {
        const string testCode =
@"
public sealed class SomeSecrets : TestSecretsContainerBase
{
    [SecretStringProperty(@""\d"", ""_test"", ↓""not constant type"")]
    public string Secret => ""1"";
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
