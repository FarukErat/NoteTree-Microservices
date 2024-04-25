using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Application.Interfaces.Infrastructure;
using ErrorOr;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Common;

public sealed class JwtHelper(
    byte[] publicKey
) : IJwtHelper
{
    private readonly byte[] _publicKey = publicKey;

    public ErrorOr<Success> VerifyToken(string token)
    {
        RSA rsa = RSA.Create();
        rsa.ImportRSAPublicKey(_publicKey, out _);

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal claimsPrincipal;
        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                }, out _);
        }
        catch (Exception e)
        {
            return Error.Unauthorized(description: e.Message);
        }

        return Result.Success;
    }

    public Dictionary<string, dynamic> DecodeToken(string token)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

        Dictionary<string, dynamic> claims = [];
        foreach (Claim claim in jwtToken.Claims)
        {
            claims.Add(claim.Type, claim.Value);
        }

        return claims;
    }

    public ErrorOr<Guid> ExtractUserId(string token)
    {
        Dictionary<string, dynamic> claims = DecodeToken(token);
        string idString = claims["nameid"];
        if (!Guid.TryParse(idString, out Guid id))
        {
            return Error.Unauthorized(description: "Invalid token");
        }

        return id;
    }
}
