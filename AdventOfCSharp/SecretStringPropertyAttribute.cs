#nullable enable

namespace AdventOfCSharp;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class SecretStringPropertyAttribute : Attribute
{
    public string Pattern { get; }
    public string Name { get; }
    public string Type { get; }

    public SecretStringPropertyAttribute(string pattern, string name, string type)
    {
        Pattern = pattern;
        Name = name;
        Type = type;
    }
}
