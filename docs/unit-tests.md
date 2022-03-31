# Unit Testing Problem Solutions

The framework provides a built-in mechanism to **automatically generate** unit tests for **all** problem solutions found in referenced projects and packages, in any of the following three widely popular unit testing frameworks of your choice:
- NUnit
- xUnit
- MSTest

This guide includes information about setting up unit tests for your solution.

## General notice

It is important that solving either part of the two of a problem should be an isolated process. In other words, solving part 2 shouldn't depend on having solved part 1.

Additionally, this feature is offered exclusively to C# projects. However, this only limits the project that contains the unit tests; you are still free to write your problem solutions in other projects in your language of preference (either VB or F#).

### Performance

Source generators generally impact performance, depending on the size of the projects, and the number of processing performed, along with the generated sources. With a bit of math, the expected generated source count should not exceed the 176 sources, 175 for each of the currently (as of Dec 2021) available days, and one for the assembly-wide configuration.

As the source generator is not an incremental one (yet), adjustments to the original problem solution project(s) will certainly pose a great hit when building the test project. Source generator support from your IDE will vary, and performance options should be available for tweaking if your development experience becomes unbearable.

## Setting up the project

Create a new .NET 6.0 unit testing project using the test framework of your choice, and add the following dependencies:
- `AdventOfCSharp.Testing.XYZ`
- `AdventOfCSharp.SourceGenerators`

where XYZ denotes any of the three offered variants per testing framework. The available testing packages are:
- `AdventOfCSharp.Testing.NUnit`
- `AdventOfCSharp.Testing.XUnit`
- `AdventOfCSharp.Testing.MSTest`

Adding multiple of these frameworks in the same project, or through indirect dependencies will not cause any issues; one of them will be "randomly" (semi-controllably deterministically) selected and perform identically to the other solutions. The only difference will be the testing framework that will be used, which might affect your test running environment if specially configured, like a CI setup or a container.

When adding the `AdventOfCSharp.SourceGenerators` dependency, edit the .csproj file to make sure that it is correctly declared like so:
```xml
<PackageReference Name="AdventOfCSharp.SourceGenerators" Version="1.4.0" OutputItemType="Analyzer"/>
```

The `OutputItemType` attribute enables the source generators to generate sources by being considered analyzers.

Do not forget to add project/package references to your problem solution projects.

## Declaring the assembly as a solution unit test assembly

This process is simplified to the core, only requiring the declaration of one attribute in the `assembly` scope of the project dedicated to unit test generation. As shown in the example below:
```csharp
using AdventOfCSharp.Testing;

[assembly: AoCSTestAssembly(AoCSTestAssemblyInfo.ProblemFileBaseDirectory)]

public static class AoCSTestAssemblyInfo
{
    // Use relative pathing in public repositories to hide actual path
    // Always prefer using absolute pathing when in a private environment
    public const string ProblemFileBaseDirectory = @"..\..\..\..\AdventOfCode";
}
```

The problem file base directory refers to the directory that contains the problem files (input and output). For convenience, this is abstracted into the attribute's constructor, wherefrom it is passed down to the generated unit tests.

### Part filtering

Unit tested parts are excluded if marked with an invalid `PartSolutionStatus`, i.e. marked as one of the below:
- `Uninitialized`
- `WIP`
- `UnavailableFreeStar`
- `Refactoring`
- `Interactive`

Additionally, part 2 of day 25 is always excluded.

## Customizing the generated tests

Currently, source generators do not enable customizing the partial generated classes. In the future, if this is supported, you will be able to individually control the problem file base directory for each of the generated unit test classes, with their respective identifiers.

## Problem solution validation

With the introduction of this feature, explicit problem solution validation from the user becomes redundant. The generated unit tests do use this approach, but the user is not required to interact with the `ProblemRunner` in such fashion.

However, problem solution validation is far from obsoletion, as it can still provide great utility in custom environments that dynamically display validation reports, in whatever way. This API still provides flexibility, and unit tests are one convenience building on top of that, replacing a more tedious approach with a cleaner one.
