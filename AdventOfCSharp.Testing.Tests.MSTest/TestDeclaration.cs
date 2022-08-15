using AdventOfCSharp.Testing;
using AdventOfCSharp.Testing.Tests.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: AoCSTestAssembly(AoCSTestAssemblyInfo.ProblemFileBaseDirectory)]

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
