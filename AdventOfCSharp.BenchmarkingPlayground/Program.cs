using AdventOfCSharp.Benchmarking;
using AdventOfCSharp.ProblemSolutionResources;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

ResourceFileManagement.SetResourceProjectAsBaseProblemFileDirectory();
ProblemBenchmarkRunner.RunAllProblems();
//ProblemBenchmarkRunner.IncludeAllProblems();

//BenchmarkSwitcher.FromTypes(new[] { typeof(ProblemBenchmark) }).Run(args, new DebugInProcessConfig());
