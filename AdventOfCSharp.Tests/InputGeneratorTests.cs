using AdventOfCSharp.Generation;
using Garyon.Extensions;
using NUnit.Framework;
using System.Linq;

namespace AdventOfCSharp.Tests;

public class InputGeneratorTests
{
    private const int numberCount = 1000;
    private const int minNumber = 1;
    private const int maxNumber = 5;

    [Test]
    public void Generate()
    {
        var generator = new DummyInputGenerator();
        var next = generator.GenerateNext();

        var lines = next.GetLines();
        var parsedNumbers = lines.Select(int.Parse).ToArray();
        Assert.AreEqual(numberCount, parsedNumbers.Length);
        foreach (var number in parsedNumbers)
            Assert.True(number is >= minNumber and <= maxNumber);
    }

    public sealed class DummyInputGenerator : InputGenerator
    {
        public override int Year => 2021;
        public override int Day => 1;

        protected override bool AlwaysGeneratesValidInputs => true;

        public override string GenerateNext()
        {
            return Integers(numberCount).Ranging(minNumber, maxNumber).PerLine().Build();
        }
    }
}
