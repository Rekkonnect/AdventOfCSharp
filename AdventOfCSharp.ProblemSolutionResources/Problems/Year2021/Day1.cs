#nullable disable

using System.Linq;

namespace AdventOfCSharp.ProblemSolutionResources.Problems.Year2021;

public class Day1 : Problem<int>
{
    private int[] numbers;

    public override int SolvePart1()
    {
        return numbers.Min() + numbers.Max();
    }
    public override int SolvePart2()
    {
        return numbers.Sum();
    }

    protected override void LoadState()
    {
        numbers = FileNumbersInt32;
    }
    protected override void ResetState()
    {
        numbers = null;
    }
}
