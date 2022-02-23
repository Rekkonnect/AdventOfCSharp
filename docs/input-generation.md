# Local Input Generation

## Summary

The framework provides a way to automatically generate sample input strings for a specific problem. A fluent API is provided for generating some basic input like integers and multi-segmented information.

## Details

### Naming Conventions

Unlike other parts of the framework, an input generator class' name does not need to follow any naming conventions, and they are not evaluated. This is why the `Year` and `Day` properties must be overridden in every implementation.

### Fluent API

Within a class inheriting `InputGenerator`, there are `InputGenerationInfo` instance initializer methods, like `Integers` and `Segmented`. In that context, they will initialize new such instances, wherefrom they can be further configured before generating the input.

Moreover, there is the ability to specify a range of values or counts that are allowed. In all ranges, both the min and the max bounds are included (**the ranges are fully inclusive**).

For example, consider generating a list of 300 to 350 integers, each ranging in [0, 100):

```csharp
var integerRule = Integers(300, 350).Ranging(0, 99).DelimitedBy(",");
```

To generate, the `Build` method is used, which returns the generated input string.

### Randomization

The randomization of the input uses the `Random.Shared` instance by default. This is configurable, and a custom `Random` instance can be used. On each of the `InputGenerationInfo` instances, the `InputGenerator` instance is respected, whose `Random` instance will be used during generation.

### Generated Input Validation

It could be up to the user to generate multiple inputs that can easily be determined as valid or not. The `IsValidInput` is a `virtual` method that determines whether an input string is valid for the given problem. Its default implementation simply runs the input and checks whether a non-`null` output is returned, and that no exceptions are thrown. The part solution status is ignored.

The `AlwaysGeneratesValidInputs` property denotes whether the generator generates valid inputs, bypassing the need for checking the validity of the input. This is so that it is not required to manually override the responsible methods to simulate that behavior.

## Examples

Consider an input generator for the [Year 2021 Day 4](https://adventofcode.com/2021/day/4) puzzle:
```csharp
public sealed class Year2021Day4InputGenerator : InputGenerator
{
    public override int Year => 2021;
    public override int Day => 4;

    public override string GenerateNext()
    {
        return Segmented
               (
                   Integers(60, 80).Ranging(0, 99).DelimitedBy(","),
                   new BingoBoardGeneratorInfo(this, 100).PerLines(2)
               ).PerLines(2).Build();
    }

    public sealed class BingoBoardGeneratorInfo : ElementSequenceInputGenerationInfo<BingoBoardGeneratorInfo.BingoBoard>
    {
        public BingoBoardGeneratorInfo(InputGenerator generator, int count)
            : base(generator, count) { }

        public override BingoBoard GenerateNextElement()
        {
            // ...
        }

        public class BingoBoard
        {
            // ...
        }
    }
}
```

The real generation rule is in the `GenerateNext()` method, denoting a two-segment input string, delimited by a double newline. The top segment is a ","-delimited list of 60 to 80 integers ranging in [0, 99], and the bottom segment is a list of 100 `BingoBoard` instances delimited by a double newline.

The `BingoBoardGeneratorInfo` will have to contain logic for generating a new `BingoBoard`.

Note: This input generator does not guarantee that the input string is valid. For this specific problem, the invalid input string rate can be estimated to be discouragingly high.