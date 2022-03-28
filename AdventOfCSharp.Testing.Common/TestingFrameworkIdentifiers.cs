using System;

namespace AdventOfCSharp.Testing;

#nullable enable

public abstract class TestingFrameworkIdentifiers
{
    public virtual string FrameworkNamePrefix => FrameworkName; 

    public abstract string FrameworkName { get; }
    public abstract string AttributeNamespace { get; }

    public abstract string InlineDataAttributeName { get; }
    public abstract string ParameterizedTestMethodAttributeName { get; }
    public abstract string? TestClassAttributeName { get; }

    public static TestingFrameworkIdentifiers? CreateFromFrameworkName(string name)
    {
        return name switch
        {
            NUnitIdentifiers.CodeName => new NUnitIdentifiers(),
            XUnitIdentifiers.CodeName => new XUnitIdentifiers(),
            MSTestIdentifiers.CodeName => new MSTestIdentifiers(),

            _ => null,
        };
    }
}

// I'm really separated between adding the dependencies to use nameof
// Or simply sticking to hard-coding the names
// Fuck dep hells man
public sealed class NUnitIdentifiers : TestingFrameworkIdentifiers
{
    public const string CodeName = "NUnit";

    public override string FrameworkName => CodeName;
    public override string AttributeNamespace => "NUnit.Framework";

    public override string InlineDataAttributeName => "TestCase";
    public override string ParameterizedTestMethodAttributeName => "Test";
    public override string? TestClassAttributeName => null;
}
public sealed class XUnitIdentifiers : TestingFrameworkIdentifiers
{
    public const string CodeName = "XUnit";

    // We have a bit of an issue with casing
    public override string FrameworkNamePrefix => CodeName;
    public override string FrameworkName => "xUnit";
    public override string AttributeNamespace => "Xunit";
    
    // Conflicts with NUnit, should not be a real scenario
    public override string InlineDataAttributeName => "InlineData";
    public override string ParameterizedTestMethodAttributeName => "Theory";
    public override string? TestClassAttributeName => null;
}
public sealed class MSTestIdentifiers : TestingFrameworkIdentifiers
{
    public const string CodeName = "MSTest";

    public override string FrameworkName => CodeName;
    public override string AttributeNamespace => "Microsoft.VisualStudio.TestTools.UnitTesting";

    public override string InlineDataAttributeName => "DataRow";
    public override string ParameterizedTestMethodAttributeName => "DataTestMethod";
    public override string? TestClassAttributeName => "TestClass";
}