﻿using AdventOfCSharp.AnalysisTestsBase.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using System;
using System.Collections.Immutable;

namespace AdventOfCSharp.AnalysisTestsBase.Verifiers;

public static class CSharpVerifierHelper
{
    /// <summary>
    /// By default, the compiler reports diagnostics for nullable reference types at
    /// <see cref="DiagnosticSeverity.Warning"/>, and the analyzer test framework defaults to only validating
    /// diagnostics at <see cref="DiagnosticSeverity.Error"/>. This map contains all compiler diagnostic IDs
    /// related to nullability mapped to <see cref="ReportDiagnostic.Error"/>, which is then used to enable all
    /// of these warnings for default validation during analyzer and code fix tests.
    /// </summary>
    internal static ImmutableDictionary<string, ReportDiagnostic> NullableWarnings { get; } = GetNullableWarningsFromCompiler();

    private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
    {
        string[] args = { "/warnaserror:nullable" };
        var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
        var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

        // Workaround for https://github.com/dotnet/roslyn/issues/41610
        nullableWarnings = nullableWarnings
            .SetItem("CS8632", ReportDiagnostic.Error)
            .SetItem("CS8669", ReportDiagnostic.Error);

        return nullableWarnings;
    }

    public static void SetupNET6AndAoCSDependencies<TVerifier>(AnalyzerTest<TVerifier> test)
        where TVerifier : IVerifier, new()
    {
        SetupSolutionTransforms(test);

        test.ReferenceAssemblies = RuntimeReferences.NET6_0Reference;

        SetupAoCSDependencies(test);
    }
    public static void SetupAoCSDependencies<TVerifier>(AnalyzerTest<TVerifier> test)
        where TVerifier : IVerifier, new()
    {
        test.TestState.AdditionalReferences.AddRange(AoCSMetadataReferences.BaseReferences);
    }
    private static void SetupSolutionTransforms<TVerifier>(AnalyzerTest<TVerifier> test)
        where TVerifier : IVerifier, new()
    {
        test.SolutionTransforms.Add((solution, projectId) =>
        {
            var compilationOptions = solution.GetProject(projectId).CompilationOptions;
            compilationOptions = compilationOptions.WithSpecificDiagnosticOptions(
                compilationOptions.SpecificDiagnosticOptions.SetItems(NullableWarnings));
            solution = solution.WithProjectCompilationOptions(projectId, compilationOptions);

            return solution;
        });
    }
}
