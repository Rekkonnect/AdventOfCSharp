namespace AdventOfCSharp.AnalysisTestsBase.Resources;

public abstract class NotProblem
{
}
public abstract class NotProblem<T1, T2> : NotProblem
{
    public abstract T1 SolvePart1();
    public abstract T2 SolvePart2();
}
public abstract class NotProblem<T> : NotProblem<T, T>
{
}
