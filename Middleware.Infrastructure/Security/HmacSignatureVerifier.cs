using System.Security.Cryptography;
using System.Text;

namespace Middleware.Infrastructure.Security;

public sealed class HmacSignatureVerifier(HmacOptions options)
{
    public bool Verify(string body, string timestamp, string signatureHex)
    {
        if (!long.TryParse(timestamp, out var ts)) return false;

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (Math.Abs(now - ts) > options.AllowedClockSkewSeconds) return false;

        // message to sign: "{timestamp}.{rawBody}"
        var message = $"{timestamp}.{body}";
        var keyBytes = Encoding.UTF8.GetBytes(options.WebhookSecret);
        var msgBytes = Encoding.UTF8.GetBytes(message);

        using var hmac = new HMACSHA256(keyBytes);
        var computed = hmac.ComputeHash(msgBytes);
        var computedHex = Convert.ToHexString(computed).ToLowerInvariant();

        // accept "sha256=<hex>" or "<hex>"
        var normalized = signatureHex.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase)
            ? signatureHex["sha256=".Length..]
            : signatureHex;

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHex),
            Encoding.UTF8.GetBytes(normalized.ToLowerInvariant())
        );
    }
}