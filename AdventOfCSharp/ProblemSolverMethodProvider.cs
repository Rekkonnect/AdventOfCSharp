
namespace AdventOfCSharp;

public static class ProblemSolverMethodProvider
{
    private const BindingFlags hiddenInstanceMember = BindingFlags.NonPublic | BindingFlags.Instance;

    private static readonly string solvePartMethodPrefix = nameof(Problem<int>.SolvePart1)[..^1];
    private static readonly string loadStateMethodName = "LoadState";
    private static readonly string resetStateMethodName = "ResetState";

    public static string SolvePartMethodName(int part) => $"{solvePartMethodPrefix}{part}";

    public static Action CreateLoadStateDelegate(Problem instance)
    {
        var method = GetLoadStateMethod(instance);
        return method.CreateDelegate<Action>(instance);
    }

    public static MethodInfo GetLoadStateMethod(Problem instance) => GetLoadStateMethod(instance.GetType());
    public static MethodInfo GetLoadStateMethod(Type problemType)
    {
        if (!problemType.Inherits<Problem>())
            throw new ArgumentException("The provided type should inherit from Problem");

        return LoadStateMethod(problemType);
    }
    public static MethodInfo GetLoadStateMethod<T>()
        where T : Problem
    {
        return GetLoadStateMethod(typeof(T));
    }

    public static Action CreateSolverDelegate(int part, Problem instance)
    {
        var method = MethodForPart(instance.GetType(), part);

        var returnType = method.ReturnType;
        var unboundFuncType = typeof(Func<>);
        var boundFuncType = unboundFuncType.MakeGenericType(returnType);
        var del = method.CreateDelegate(boundFuncType, instance);
        var action = VeryUnsafe.VeryUnsafe.ChangeType<Action>(del);
        return action;
    }

    public static MethodInfo MethodForPart(Type problemType, int part)
    {
        return problemType.GetMethod(SolvePartMethodName(part))!;
    }
    public static MethodInfo MethodForPart<T>(int part)
        where T : Problem
    {
        return MethodForPart(typeof(T), part);
    }
    public static MethodInfo MethodForPart(int part) => MethodForPart<Problem<int>>(part);
    public static MethodInfo[] MethodsForOfficialParts(Type problemType) => new[] { MethodForPart(problemType, 1), MethodForPart(problemType, 2) };
    public static MethodInfo[] MethodsForOfficialParts() => MethodsForOfficialParts(typeof(Problem));

    public static MethodInfo LoadStateMethod(Type problemType) => PrivateMethod(problemType, loadStateMethodName);
    public static MethodInfo LoadStateMethod() => LoadStateMethod(typeof(Problem));
    public static MethodInfo ResetStateMethod(Type problemType) => PrivateMethod(problemType, resetStateMethodName);
    public static MethodInfo ResetStateMethod() => ResetStateMethod(typeof(Problem));

    private static MethodInfo PrivateMethod(Type problemType, string name) => problemType.GetMethod(name, hiddenInstanceMember)!;
    private static MethodInfo PrivateMethod<T>(string name)
        where T : Problem
    {
        return typeof(T).GetMethod(name, hiddenInstanceMember)!;
    }
    private static MethodInfo PrivateMethod(string name) => PrivateMethod<Problem>(name);

    public static MethodInfo[] PartSolverMethods(Type type) => type.GetMethods().Where(m => m.HasCustomAttribute<PartSolverAttribute>()).ToArray();
}
