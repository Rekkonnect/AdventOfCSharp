using AdventOfCSharp.AnalysisTestsBase;
using AdventOfCSharp.AnalysisTestsBase.Verifiers;
using AdventOfCSharp.Analyzers;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RoseLynn.Analyzers;
using RoseLynn.Testing;
using System.Threading.Tasks;

namespace AdventOfCSharp.CodeFixes.Tests;

public abstract class BaseCodeFixTests<TAnalyzer, TCodeFix> : BaseCodeFixDiagnosticTests<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : AoCSCodeFixProvider, new()
{
    protected sealed override DiagnosticDescriptorStorageBase DiagnosticDescriptorStorage => AoCSDiagnosticDescriptorStorage.Instance;

    protected sealed override async Task VerifyCodeFixAsync(string markupCode, string expected, int codeActionIndex)
    {
        await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyCodeFixAsync(markupCode, expected, codeActionIndex);
    }

    [TestMethod]
    public void TestExistingCodeFixName()
    {
        Assert.IsNotNull(new TCodeFix().CodeFixTitle);
    }

    public void TestCodeFixWithUsings(string markupCode, string expected, int codeActionIndex = 0)
    {
        TestCodeFix(AoCSUsingsProvider.Instance.WithUsings(markupCode), AoCSUsingsProvider.Instance.WithUsings(expected), codeActionIndex);
    }
}
