#nullable enable

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCSharp;

// TODO: Revisit nullability
public abstract class AnswerStringConverter
{
    /// <param name="value">The value to convert.</param>
    /// <returns>The string result that is to be given on the website.</returns>
    /// <remarks>Do NOT use unless you do not know the type of the converter that will be used.</remarks>
    public string ConvertObject(object value)
    {
        var convertMethods = GetType().GetMethods().Where(method => method.Name == nameof(ObjectAnswerStringConverter.Convert));
        var convertMethod = convertMethods.First(HasDesiredParameter);
        return (convertMethod.Invoke(this, new[] { value }) as string)!;

        static bool HasDesiredParameter(MethodInfo method)
        {
            var parameters = method.GetParameters();
            if (parameters.Length != 1)
                return false;

            var declaringType = method.GetBaseDefinition().DeclaringType;
            Debug.Assert(declaringType!.GetGenericTypeDefinition() == typeof(AnswerStringConverter<>));
            return declaringType.GenericTypeArguments[0] == parameters[0].ParameterType;
        }
    }
}
public abstract class AnswerStringConverter<TSource> : AnswerStringConverter
{
    public bool CanConvert(TSource value, [NotNullWhen(true)] out string converted)
    {
        return (converted = Convert(value)) is not null;
    }

    public abstract string Convert(TSource value);
}

public abstract class ObjectAnswerStringConverter : AnswerStringConverter<object> { }

public sealed class CommonAnswerStringConverter : ObjectAnswerStringConverter
{
    public static CommonAnswerStringConverter Instance { get; } = new();

    public override string Convert(object value) => value.ToString()!;
}