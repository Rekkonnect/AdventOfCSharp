#nullable enable

using System.Net.Http;

namespace AdventOfCSharp;

/// <summary>Represents an uninitialized cookie container for performing input requests.</summary>
/// <remarks>WARNING: Do not eat! Santa will be sad!</remarks>
public abstract class Cookies : ISecretsContainer
{
    public abstract string GA { get; }
    public abstract string Session { get; }

    public void AddToDefaultRequestHeaders(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("cookie", ToString());
    }

    public override string ToString()
    {
        return $"_ga={GA}; session={Session}";
    }
}
