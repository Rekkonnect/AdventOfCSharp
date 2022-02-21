using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.SecretStringProperties;

[TestClass]
public sealed class AoCS0084_Tests : SecretStringPropertyAnalyzerTests
{
    [TestMethod]
    public void NotSecretsContainerType()
    {
        string testCode =
@"
public class NotSecretContainer
{
    public const string SecretsType = ""Test"";

    [↓SecretStringProperty(@""\d"", ""_test"", SecretsType)]
    public string Secret => ""1"";
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
