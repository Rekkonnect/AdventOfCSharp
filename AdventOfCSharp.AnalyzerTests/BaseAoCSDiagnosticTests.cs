using AdventOfCSharp.AnalysisTestsBase;
using AdventOfCSharp.AnalysisTestsBase.Verifiers;
using Gu.Roslyn.Asserts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoseLynn.Analyzers;
using RoseLynn.Testing;

namespace AdventOfCSharp.Analyzers.Tests;

public abstract class BaseAoCSDiagnosticTests<TAnalyzer> : BaseAoCSDiagnosticTests
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    protected sealed override DiagnosticAnalyzer GetNewDiagnosticAnalyzerInstance() => new TAnalyzer();

    // This API was stolen from RoseLynn; consider extending this multi-framework approach
    private void ReplaceAsteriskMarkup(ref string markupCode)
    {
        markupCode = ReplaceAsteriskMarkup(markupCode);
    }
    private string ReplaceAsteriskMarkup(string markupCode)
    {
        return markupCode.Replace("{|*", $"{{|{TestedDiagnosticRule.Id}");
    }

    protected void AssertOrValidateMicrosoftCodeAnalysis(string testCode, bool assert)
    {
        if (assert)
        {
            AssertDiagnosticsWithUsings(testCode);
        }
        else
        {
            ReplaceAsteriskMarkup(ref testCode);
            testCode = DiagnosticMarkupCodeHandler.MicrosoftCodeAnalysis.RemoveMarkup(testCode);
            ValidateCodeWithUsings(testCode);
        }
    }

    // Some extensibility can be extracted from this API too
    protected void AssertDiagnosticsMicrosoftCodeAnalysis(string testCode)
    {
        ReplaceAsteriskMarkup(ref testCode);
        CSharpAnalyzerVerifier<TAnalyzer>.VerifyAnalyzerAsync(testCode).Wait();
    }
}

public abstract class BaseAoCSDiagnosticTests : BaseDiagnosticTests
{
    protected ExpectedDiagnostic ExpectedDiagnostic => ExpectedDiagnostic.Create(TestedDiagnosticRule);
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => AoCSDiagnosticDescriptorStorage.Instance;

    // First time using this pattern to seal overriding, special syntax sugar would be appreciated
    public sealed override DiagnosticDescriptor TestedDiagnosticRule => base.TestedDiagnosticRule;

    protected sealed override UsingsProviderBase GetNewUsingsProviderInstance()
    {
        return AoCSUsingsProvider.Instance;
    }

    protected sealed override void ValidateCode(string testCode)
    {
        RoslynAssert.Valid(GetNewDiagnosticAnalyzerInstance(), testCode);
    }
    protected override void AssertDiagnostics(string testCode)
    {
        RoslynAssert.Diagnostics(GetNewDiagnosticAnalyzerInstance(), ExpectedDiagnostic, testCode);
    }

    [TestMethod]
    public void EmptyCode()
    {
        ValidateCode("");
    }
    [TestMethod]
    public void EmptyCodeWithUsings()
    {
        ValidateCodeWithUsings("");
    }
}
