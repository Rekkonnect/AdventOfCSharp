#nullable enable

using System.Net.Http;

namespace AdventOfCSharp;

/// <summary>Represents an uninitialized cookie container for performing input requests.</summary>
/// <remarks>WARNING: Do not eat! Santa will be sad!</remarks>
public abstract class Cookies : ISecretsContainer
{
    private const string gaPattern = @"GA\d\.\d\.\d{10}\.\d{10}";
    private const string sessionPattern = @"[0-9a-f]{128}";

    private const string cookieName = "cookie";

    [SecretStringProperty(gaPattern, "_ga", cookieName)]
    public abstract string GA { get; }
    [SecretStringProperty(sessionPattern, "session", cookieName)]
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
