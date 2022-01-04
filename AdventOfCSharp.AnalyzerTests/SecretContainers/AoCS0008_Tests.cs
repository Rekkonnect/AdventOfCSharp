using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.SecretContainers;

[TestClass]
public sealed class AoCS0008_Tests : SecretsContainerAnalyzerTests
{
    [TestMethod]
    public void TemplateSecretsContainer()
    {
        const string testCode =
@"
namespace AoC;

[SecretsContainer]
internal sealed class MyCookies : Cookies
{
    public override string GA => ""GA1.2.9999999999.9999999999"";
    public override string Session => ""ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff"";
}
";

        ValidateCodeWithUsings(testCode);
    }

    [TestMethod]
    public void InvalidSecretsContainer()
    {
        const string testCode =
@"
namespace AoC;

[↓SecretsContainer]
public abstract class SomeSecrets : ISecretsContainer { }
public abstract class SomeNotSecrets { }

[SecretsContainer]
internal sealed class MySecrets : SomeSecrets { }

[↓SecretsContainer]
internal sealed class InvalidConstructor : SomeSecrets
{
    public InvalidConstructor(int x) { }
}

[↓SecretsContainer]
internal class NotSealedSecrets : SomeSecrets { }

[↓SecretsContainer]
internal sealed class NotSecrets : SomeNotSecrets { }
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
