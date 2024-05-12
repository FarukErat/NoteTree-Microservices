using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces.Infrastructure;
using ErrorOr;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Infrastructure.Common;

public class JwtVerifier(
    IPublicKeyProvider publicKeyProvider
) : IJwtVerifier
{
    private readonly IPublicKeyProvider _publicKeyProvider = publicKeyProvider;

    public Guid? ExtractUserId(string token)
    {
        Dictionary<string, dynamic> claims = GetTokenClaims(token);

        if (claims.TryGetValue("sub", out dynamic? userIddynamic))
        {
            if (userIddynamic is string userIdString)
            {
                if (Guid.TryParse(userIdString, out Guid userIdGuid))
                {
                    return userIdGuid;
                }
            }
        }

        return null;
    }

    public ErrorOr<Success> VerifyToken(string token, string audience)
    {
        Dictionary<string, dynamic> claims = GetTokenClaims(token);

        string? audienceClaim = claims.GetValueOrDefault("aud");
        if (audienceClaim != audience)
        {
            return Error.Validation(description: "Token audience does not match the expected audience.");
        }

        string? keyId = claims.GetValueOrDefault("kid");
        if (keyId is null)
        {
            return Error.Validation(description: "Token does not contain a key ID.");
        }

        byte[]? publicKey = _publicKeyProvider.GetPublicKeyById(keyId);
        if (publicKey is null)
        {
            return Error.Validation(description: "Public key not found.");
        }

        RSA rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal claimsPrincipal;
        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = Configurations.Jwt.Issuer,

                    ValidateAudience = false, // Audience is validated manually
                    ValidAudience = audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    IssuerSigningKey = new RsaSecurityKey(rsa)
                }, out _);
        }
        catch (Exception e)
        {
            return Error.Validation(description: e.Message);
        }

        return Result.Success;
    }

    private static Dictionary<string, dynamic> GetTokenClaims(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return [];
        }

        string[] tokenParts = token.Split('.');
        if (tokenParts.Length != 3)
        {
            return [];
        }

        Dictionary<string, dynamic> headerClaims = GetTokenPartClaims(tokenParts[0]);
        Dictionary<string, dynamic> payloadClaims = GetTokenPartClaims(tokenParts[1]);
        Dictionary<string, dynamic> result = headerClaims;
        foreach (KeyValuePair<string, dynamic> pair in payloadClaims)
        {
            result[pair.Key] = pair.Value;
        }

        return result;
    }

    private static Dictionary<string, dynamic> GetTokenPartClaims(string part)
    {
        if (string.IsNullOrEmpty(part))
        {
            return [];
        }

        part += new string('=', (4 - (part.Length % 4)) % 4); // add base64 padding

        byte[] partBytes;
        try
        {
            // may not be a valid base64 string
            partBytes = Convert.FromBase64String(part);
        }
        catch
        {
            return [];
        }

        string partJson = Encoding.UTF8.GetString(partBytes);
        return JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(partJson) ?? [];
    }
}
