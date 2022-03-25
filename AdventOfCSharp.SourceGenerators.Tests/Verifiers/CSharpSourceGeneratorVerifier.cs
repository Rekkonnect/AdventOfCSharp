using AdventOfCSharp.AnalysisTestsBase.Verifiers;
using AdventOfCSharp.SourceGenerators.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using System;
using System.Collections.Immutable;

namespace AdventOfCSharp.SourceGenerators.Tests.Verifiers;

public static class CSharpSourceGeneratorVerifier<TSourceGenerator>
    where TSourceGenerator : ISourceGenerator, new()
{
    public class Test : CSharpSourceGeneratorTest<TSourceGenerator, NUnitVerifier>
    {
        public Test()
        {
            CSharpVerifierHelper.SetupNET6AndAoCSDependencies(this);
            TestState.AdditionalReferences.AddRange(BenchmarkSpecificMetadataReferences.BaseBenchmarkReferences);
        }

        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = base.CreateCompilationOptions();
            return compilationOptions.WithSpecificDiagnosticOptions(
                 compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
        }

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            string[] args = { "/warnaserror:nullable" };
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
            var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

            return nullableWarnings;
        }

        protected override ParseOptions CreateParseOptions()
        {
            return (base.CreateParseOptions() as CSharpParseOptions).WithLanguageVersion(LanguageVersion);
        }
    }
}
