# Benchmarking Problem Solutions

The framework provides a built-in mechanism to **generate** benchmarks using [Benchmark.NET](https://benchmarkdotnet.org/). This guide includes information about declaring and running benchmarks for your solutions.

## General notice

It is important that solving either part of the two of a problem should be an isolated process. In other words, solving part 2 shouldn't depend on having solved part 1.

Additionally, this feature is offered exclusively to C# projects. However, this only limits the project that contains the benchmarks; you are still free to write your problem solutions in other projects in your language of preference (either VB or F#).

## Setting up the project

Create a new .NET 6.0 Console Application project, using the following dependencies:
- `BenchmarkDotNet`
- `AdventOfCSharp.Benchmarking`
- `AdventOfCSharp.SourceGenerators`

When adding the `AdventOfCSharp.SourceGenerators` dependency, edit the .csproj file to make sure that it is correctly declared like so:
```xml
<PackageReference Name="AdventOfCSharp.SourceGenerators" Version="1.4.0" OutputItemType="Analyzer"/>
```

The `OutputItemType` attribute enables the source generators to generate sources by being considered analyzers.

## Describing the benchmarks

The benchmarking class must be declared a benchmark describer, using the `BenchmarkDescriberAttribute`. The class must be declared partial and **not** sealed.

Additionally, a combination of benchmark filtering attributes must be used, from the ones below:
- `AllDates`: all the available dates will be included
- `Days`: all available dates with the given days will be included
- `Years`: all dates within the specified years will be included
- `Dates`: the specified dates will be included, if found

The filters work additively, as a union. Therefore, `AllDates` renders all other attributes useless.

### Examples

Generate benchmarks for all available dates:
```csharp
[BenchmarkDescriber]
[AllDates]
public partial class AllDescriber { }
```

Only generate for the entire years 2015 and 2021, including 2019/24, 2020/24 and 2020/25:
```csharp
[BenchmarkDescriber]
[Years(2015, 2021)]
[Dates(2019, 24)]
[Dates(2020, 24, 25)]
public partial class ExampleDescriber { }
```

### Benchmarked parts

Additionally, you may filter out part solutions, or include benchmarking the `LoadInput()` method, using the `PartsAttribute`, like in the example:

```csharp
[BenchmarkDescriber]
[AllDates]
[Parts(BenchmarkingParts.Input | BenchmarkingParts.Part2)]
public partial class ExampleDescriber { }
```

The above describer declares that only the `LoadInput` and `SolvePart2` methods will be benchmarked.

### Part filtering

Like generated unit tests, benchmarked parts are excluded if marked with an invalid `PartSolutionStatus`, i.e. marked as one of the below:
- `Uninitialized`
- `WIP`
- `UnavailableFreeStar`
- `Refactoring`
- `Interactive`

Additionally, part 2 of day 25 is always excluded.

## Running the benchmarks

The input files will not be automatically found; you will have to set the directory up. For public projects, it is heavily recommended to do so using relative paths, like the example below.

When running the benchmarks, the base input file directory is **not** read by setting the `ProblemFiles.CustomBaseDirectory` property; environment variables are used for connecting between the processes, as Benchmark.NET uses separate processes when running the benchmarks.

When running the benchmarks, you are asked to specify a directory wherefrom the problem files will be read. For example,

```csharp
const string baseInputFileDirectory = @"..\..\..\..\AoC.Example";
BenchmarkDescriberExecution.RunBenchmark<AllDescriber>(baseInputFileDirectory);
```

The relative path shown could vary depending on your project's structure.
