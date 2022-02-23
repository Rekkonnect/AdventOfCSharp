using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCSharp.Analyzers.Tests.SecretStringProperties;

[TestClass]
public sealed class CookiesTests : SecretStringPropertyAnalyzerTests
{
    public override DiagnosticDescriptor TestedDiagnosticRule => AoCSDiagnosticDescriptorStorage.Instance[0080];

    [TestMethod]
    public void CorrectCookies()
    {
        // The cookies below are random mutations of existing ones
        // Do not attempt to use them, even if they are valid for somebody
        const string testCode =
@"
[SecretsContainer]
internal sealed class MyCookies : Cookies
{
    public override string GA => ""GA1.2.2949042032.1125072387"";
    public override string Session => ""04f0b5c0e6f05ca60d05b605a05468e056b1a056c026e156f05b605e0a202c050f050023b02e023a020c99833fe45c541c7b3af6b623d95718f2f9a23ecd7629"";
}
";

        ValidateCodeWithUsings(testCode);
    }

    [TestMethod]
    public void IncorrectCookies()
    {
        // The cookies below are random mutations of existing ones
        // Do not attempt to use them, even if they are valid for somebody
        const string testCode =
@"
[SecretsContainer]
internal sealed class MyCookies : Cookies
{
    public override string GA => ↓""fgjdsrignjudtsignxzudi"";
    public override string Session => ↓""jhnbdsfgjhkgbfdsnudfjvnsjudtrignsudigndsikjugtn"";
}
";

        AssertDiagnosticsWithUsings(testCode);
    }
}
