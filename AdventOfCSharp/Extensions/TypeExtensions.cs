namespace AdventOfCSharp.Extensions;

public static class TypeExtensions
{
    public static bool InheritsGenericDefinition(this Type type, Type genericType, out Type[] typeArguments)
    {
        if (!genericType.IsGenericTypeDefinition)
            throw new ArgumentException("The generic type must be a generic type definition, that is, it should not have any substituted type parameters.");

        if (!type.IsClass && !type.IsInterface)
            throw new ArgumentException("Only classes and interfaces can inherit from generic types.");

        if (genericType.IsInterface && type.IsClass)
            throw new ArgumentException("An interface cannot inherit from a class.");

        return InheritsGenericDefinitionUnsafe(type, genericType, out typeArguments);
    }
    private static bool InheritsGenericDefinitionUnsafe(Type type, Type genericType, out Type[] typeArguments)
    {
        typeArguments = null!;

        var baseType = type;

        while (baseType is not null)
        {
            // Garyon contains a bug here for sure
            if (baseType.IsGenericVariantOf(genericType))
            {
                typeArguments = baseType.GenericTypeArguments;
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }
}
