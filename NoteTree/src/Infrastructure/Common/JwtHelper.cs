using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Application.Interfaces.Infrastructure;
using Domain.Enums;
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

    public Guid? ExtractUserId(string token)
    {
        Dictionary<string, dynamic> claims = DecodeToken(token);
        string idString = claims["nameid"];
        if (!Guid.TryParse(idString, out Guid id))
        {
            return null;
        }

        return id;
    }

    public List<Role>? GetUserRoles(string token)
    {
        Dictionary<string, dynamic> claims = DecodeToken(token);
        string rolesString = claims["role"];

        List<string>? stringList = JsonSerializer.Deserialize<List<string>>(rolesString);
        if (stringList is null)
        {
            return null;
        }

        List<Role> roles = [];
        foreach (string role in stringList)
        {
            if (Enum.TryParse(role, out Role roleEnum))
            {
                roles.Add(roleEnum);
            }
        }

        return roles;
    }
}
