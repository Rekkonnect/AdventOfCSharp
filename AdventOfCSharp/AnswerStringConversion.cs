#nullable enable

using AdventOfCSharp.Extensions;

namespace AdventOfCSharp;

public static class AnswerStringConversion
{
    private static readonly TypeConverterDictionary typeConverterDictionary;

    static AnswerStringConversion()
    {
        typeConverterDictionary = new();

        var classes = AppDomainCache.Current.AllNonAbstractClasses;
        foreach (var type in classes)
        {
            if (type == typeof(CommonAnswerStringConverter))
                continue;

            if (type.InheritsGenericDefinition(typeof(AnswerStringConverter<>), out var typeArguments))
            {
                var convertedType = typeArguments[0];
                typeConverterDictionary.Add(convertedType, type);
            }
        }
    }

    public static string Convert(object value)
    {
        var converter = typeConverterDictionary.GetInitializedConverterForType(value.GetType());
        return converter.ConvertObject(value);
    }

    private class TypeConverterDictionary
    {
        private readonly FlexibleDictionary<Type, Type> dictionary = new();
        private readonly List<Type> objectConverters = new();

        public IEnumerable<Type> ConvertableTypes => dictionary.Keys;
        public IEnumerable<Type> Converters => dictionary.Values;

        public void Add(Type convertedType, Type converter)
        {
            if (convertedType == typeof(object))
                objectConverters.Add(converter);
            else
                dictionary.Add(convertedType, converter);
        }

        // TODO: Support returning object converters in the future
        public Type GetConverterForType(Type convertedType) => GetDeclaredConverterForType(convertedType) ?? typeof(CommonAnswerStringConverter);
        public AnswerStringConverter GetInitializedConverterForType(Type convertedType)
        {
            return GetDeclaredConverterForType(convertedType)?.InitializeInstance<AnswerStringConverter>() ?? CommonAnswerStringConverter.Instance;
        }

        private Type? GetDeclaredConverterForType(Type convertedType)
        {
            var currentConvertedType = convertedType;
            while (currentConvertedType is not null)
            {
                if (dictionary.TryGetValue(currentConvertedType, out var result))
                    return result;

                currentConvertedType = currentConvertedType.BaseType;
            }

            foreach (var baseInterface in convertedType.GetInterfaces())
            {
                if (dictionary.TryGetValue(baseInterface, out var result))
                    return result;
            }

            return null;
        }
    }
}
