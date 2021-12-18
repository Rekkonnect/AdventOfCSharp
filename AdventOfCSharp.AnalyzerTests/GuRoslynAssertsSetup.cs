using Gu.Roslyn.Asserts;
using System.Runtime.CompilerServices;

namespace AdventOfCSharp.Analyzers.Tests;

public static class GuRoslynAssertsSetup
{
    [ModuleInitializer]
    public static void Setup()
    {
        Settings.Default = Settings.Default
            .WithAllowedCompilerDiagnostics(AllowedCompilerDiagnostics.WarningsAndErrors)
            .WithMetadataReferences(MetadataReferences.Transitive(typeof(GuRoslynAssertsSetup)));
    }
}