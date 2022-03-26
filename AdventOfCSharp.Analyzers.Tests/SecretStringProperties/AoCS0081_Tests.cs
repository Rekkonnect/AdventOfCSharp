using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.SecretStringProperties;

[TestClass]
public sealed class AoCS0081_Tests : SecretStringPropertyAnalyzerTests
{
    [TestMethod]
    public void WithoutSecretStringProperties()
    {
        const string testCode =
@"
public sealed class ↓NoSecrets : NoSecretsBase
{
    public string NotSecretA => ""nothing"";
    public string NotSecretB => ""random"";
    public override int Value => 5;
}
public abstract class NoSecretsBase : TestSecretsContainerBase
{
    public string NotSecretBase => ""base"";
    public abstract int Value { get; }
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
