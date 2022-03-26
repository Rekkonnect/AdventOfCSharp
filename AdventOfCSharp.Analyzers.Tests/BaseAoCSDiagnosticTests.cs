using AdventOfCSharp.AnalysisTestsBase;
using AdventOfCSharp.AnalysisTestsBase.Verifiers;
using Gu.Roslyn.Asserts;
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

    [TestMethod]
    public void IrrelevantDeclarations()
    {
        const string code =
@"
public sealed class A { }
public sealed class A<T1> : A { }
public sealed class A<T1, T2> : A<T1> { }
public sealed class A<T1, T2, T3> : A<T1, T2> { }

public struct S : I { }
public struct S<T1> : I<T1> { }
public struct S<T1, T2> : I<T1, T2> { }
public struct S<T1, T2, T3> : I<T1, T2, T3> { }

public interface I { }
public interface I<T1> { }
public interface I<T1, T2> { }
public interface I<T1, T2, T3> { }

public enum E { }
public enum F { }

public delegate void D();
public delegate int RD(int a);
";

        ValidateCodeWithUsings(code);
    }
}
