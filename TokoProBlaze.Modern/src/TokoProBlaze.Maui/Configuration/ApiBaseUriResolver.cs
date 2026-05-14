using Microsoft.Extensions.Configuration;

namespace TokoProBlaze.Maui.Configuration;

internal static class ApiBaseUriResolver
{
    /// <summary>
    /// Menormalisasi URL API. Pada Android emulator, <c>localhost</c> mengarah ke perangkat itu sendiri;
    /// host development biasanya <c>10.0.2.2</c>.
    /// </summary>
    public static Uri Resolve(IConfiguration configuration)
    {
        var raw = configuration["ApiBaseUrl"]?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            raw = "http://localhost:5252";
        }

        if (!Uri.TryCreate(raw, UriKind.Absolute, out var uri))
        {
            return new Uri("http://localhost:5252/", UriKind.Absolute);
        }

#if ANDROID
        if (string.Equals(uri.Host, "localhost", StringComparison.OrdinalIgnoreCase)
            || string.Equals(uri.Host, "127.0.0.1", StringComparison.Ordinal))
        {
            var builder = new UriBuilder(uri) { Host = "10.0.2.2" };
            return EnsureTrailingSlash(builder.Uri);
        }
#endif
        return EnsureTrailingSlash(uri);
    }

    private static Uri EnsureTrailingSlash(Uri uri)
    {
        var s = uri.ToString();
        return s.EndsWith('/', StringComparison.Ordinal) ? uri : new Uri(s + "/", UriKind.Absolute);
    }
}
