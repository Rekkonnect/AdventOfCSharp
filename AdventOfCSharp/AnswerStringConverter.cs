namespace AdventOfCSharp;

public abstract class AnswerStringConverter
{
    /// <param name="value">The value to convert. Its type must strictly match the target type of the converter.</param>
    /// <returns>The string result that is to be given on the website.</returns>
    /// <remarks>Do NOT use unless you do not know the type of the converter that will be used.</remarks>
    public string? ConvertObject(object value)
    {
        var convertMethod = GetType().GetMethods().Single(method => method.HasCustomAttribute<ConvertMethodAttribute>());
        return convertMethod.Invoke(this, new[] { value }) as string;
    }
}
public abstract class AnswerStringConverter<TSource> : AnswerStringConverter
{
    [ConvertMethod]
    public abstract string? Convert(TSource value);
}

public sealed class CommonAnswerStringConverter : AnswerStringConverter<object>
{
    public static CommonAnswerStringConverter Instance { get; } = new();

    public override string Convert(object value) => value.ToString()!;
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
internal sealed class ConvertMethodAttribute : Attribute { }