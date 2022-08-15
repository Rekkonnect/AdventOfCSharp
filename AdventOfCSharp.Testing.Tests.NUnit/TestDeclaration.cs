using AdventOfCSharp.Testing;
using AdventOfCSharp.Testing.Tests.Common;
using NUnit.Framework;

[assembly: AoCSTestAssembly(AoCSTestAssemblyInfo.ProblemFileBaseDirectory)]

[assembly: Parallelizable(ParallelScope.Children)]