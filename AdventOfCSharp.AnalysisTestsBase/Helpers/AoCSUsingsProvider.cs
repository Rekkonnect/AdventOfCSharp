using RoseLynn.Testing;

namespace AdventOfCSharp.AnalysisTestsBase;

public sealed class AoCSUsingsProvider : UsingsProviderBase
{
    public static readonly AoCSUsingsProvider Instance = new();

    public const string DefaultUsings =
@"
using AdventOfCSharp;
using AdventOfCSharp.AnalysisTestsBase.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
";

    public override string DefaultNecessaryUsings => DefaultUsings;

    private AoCSUsingsProvider() { }
}
