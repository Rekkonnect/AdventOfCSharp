using AdventOfCSharp.AnalysisTestsBase.Resources;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using RoseLynn;
using System.IO;

namespace AdventOfCSharp.AnalysisTestsBase.Verifiers;

public static partial class CSharpCodeFixVerifier<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    public class Test : CSharpCodeFixTest<TAnalyzer, TCodeFix, MSTestVerifier>
    {
        public Test()
        {
            SolutionTransforms.Add((solution, projectId) =>
            {
                var compilationOptions = solution.GetProject(projectId).CompilationOptions;
                compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                    compilationOptions.SpecificDiagnosticOptions.SetItems(CSharpVerifierHelper.NullableWarnings));
                solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

                return solution;
            });

            // This is an absolute fucking disgrace
            ReferenceAssemblies = new ReferenceAssemblies(
                "net6.0",
                new PackageIdentity(
                    "Microsoft.NETCore.App.Ref", "6.0.0"),
                    Path.Combine("ref", "net6.0"));

            TestState.AdditionalReferences.AddRange(new[]
            {
                MetadataReferenceFactory.CreateFromType<Problem>(),
                MetadataReferenceFactory.CreateFromType<ExampleAttribute>(),
            });
        }
    }
}
