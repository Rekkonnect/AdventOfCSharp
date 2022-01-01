using AdventOfCSharp.AnalysisTestsBase;
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
    protected sealed override void AssertDiagnostics(string testCode)
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
