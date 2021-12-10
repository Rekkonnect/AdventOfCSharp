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
        var specializedConverted = ConvertUsingSpecializedTypeConverter(value);
        if (specializedConverted is not null)
            return specializedConverted;

        return ConvertUsingFirstValidObjectConverter(value);
    }
    private static string? ConvertUsingSpecializedTypeConverter(object value)
    {
        var converter = typeConverterDictionary.GetInitializedConverterForType(value.GetType());
        return converter?.ConvertObject(value);
    }
    private static string ConvertUsingFirstValidObjectConverter(object value)
    {
        foreach (var converter in typeConverterDictionary.ObjectConverters)
        {
            var converted = converter.InitializeInstance<AnswerStringConverter>().ConvertObject(value);
            if (converted is not null)
                return converted;
        }
        return CommonAnswerStringConverter.Instance.Convert(value);
    }

    private class TypeConverterDictionary
    {
        // TODO: Consider deprecating object converters
        private readonly FlexibleDictionary<Type, Type> dictionary = new();
        private readonly List<Type> objectConverters = new();

        public IEnumerable<Type> ConvertableTypes => dictionary.Keys;
        public IEnumerable<Type> Converters => dictionary.Values;

        public IReadOnlyCollection<Type> ObjectConverters => objectConverters;

        public void Add(Type convertedType, Type converter)
        {
            if (convertedType == typeof(object))
                objectConverters.Add(converter);
            else
                dictionary.Add(convertedType, converter);
        }

        // TODO: Support returning object converters in the future
        public Type? GetConverterForType(Type convertedType) => GetDeclaredOrDeterminedConverterForType(convertedType);
        public AnswerStringConverter? GetInitializedConverterForType(Type convertedType)
        {
            return GetDeclaredOrDeterminedConverterForType(convertedType)?.InitializeInstance<AnswerStringConverter>();
        }

        private Type? GetDeclaredOrDeterminedConverterForType(Type convertedType)
        {
            if (IsHandledBuiltInType(convertedType))
                return typeof(CommonAnswerStringConverter);
            return GetDeclaredConverterForType(convertedType);
        }
        private static bool IsHandledBuiltInType(Type type)
        {
            return type.GetTypeCode()
                is TypeCode.Byte
                or TypeCode.Int16
                or TypeCode.Int32
                or TypeCode.Int64
                or TypeCode.SByte
                or TypeCode.UInt16
                or TypeCode.UInt32
                or TypeCode.UInt64
                or TypeCode.Single
                or TypeCode.Double
                or TypeCode.Decimal
                or TypeCode.String;
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
