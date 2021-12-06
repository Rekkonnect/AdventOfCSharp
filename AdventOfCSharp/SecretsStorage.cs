#nullable enable

using TypeEx = Garyon.Reflection.TypeExtensions;
using MemberInfoEx = Garyon.Reflection.MemberInfoExtensions;

namespace AdventOfCSharp;

public static class SecretsStorage
{
    private static bool searchedCookies = false;
    private static Cookies? cookies;

    public static Cookies? Cookies
    {
        get
        {
            if (searchedCookies)
                return cookies;

            searchedCookies = true;
            return cookies = GetCookies();
        }
    }

    private static Cookies? GetCookies()
    {
        var cookiesClasses = AppDomainCache.Current.AllNonAbstractClasses.Where(TypeEx.Inherits<Cookies>);

        return cookiesClasses.FirstOrDefault(MemberInfoEx.HasCustomAttribute<SecretsContainerAttribute>)
                            ?.InitializeInstance<Cookies>();
    }
}
