using System;

namespace AdventOfCSharp;

/// <summary>Decorates a class to denote that it acts as a container for secrets.</summary>
public interface ISecretsContainer { }

/// <summary>Marks a class as a secrets container from which information should be retrieved.</summary>
/// <remarks>It is important that the marked class implements the <seealso cref="ISecretsContainer"/> interface.</remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class SecretsContainerAttribute : Attribute
{
}
