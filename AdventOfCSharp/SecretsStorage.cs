#nullable enable

using TypeEx = Garyon.Reflection.TypeExtensions;
using MemberInfoEx = Garyon.Reflection.MemberInfoExtensions;

namespace AdventOfCSharp;

/// <summary>Provides the default instances for important <seealso cref="ISecretsContainer"/> types.</summary>
public static class SecretsStorage
{
    private static readonly Lazy<Cookies?> lazyCookies = new(GetCookies);

    public static Cookies? Cookies => lazyCookies.Value;

    private static Cookies? GetCookies()
    {
        var cookiesClasses = AppDomainCache.Current.AllNonAbstractClasses.Where(TypeEx.Inherits<Cookies>);

        return cookiesClasses.FirstOrDefault(MemberInfoEx.HasCustomAttribute<SecretsContainerAttribute>)
                            ?.InitializeInstance<Cookies>();
    }
}
