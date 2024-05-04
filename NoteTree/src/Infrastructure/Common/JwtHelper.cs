using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Application.Interfaces.Infrastructure;
using Domain.Enums;
using ErrorOr;
using Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Common;

public sealed class JwtHelper(
    GetVerificationKeyService getVerificationKeyService
) : IJwtHelper
{
    private readonly GetVerificationKeyService _getVerificationKeyService = getVerificationKeyService;

    public async Task<ErrorOr<Success>> VerifyTokenAsync(string token)
    {
        ErrorOr<byte[]> publicKeyResult = await GetKeyByJwtAsync(token);
        if (publicKeyResult.IsError)
        {
            return publicKeyResult.FirstError;
        }

        byte[] publicKey = publicKeyResult.Value;
        RSA rsa = RSA.Create();
        rsa.ImportRSAPublicKey(publicKey, out _);

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal claimsPrincipal;
        // TODO: consider validating audience with request ip so that tokens can't be used from other ips
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

        foreach (KeyValuePair<string, object> header in jwtToken.Header)
        {
            claims.Add(header.Key, header.Value);
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

    private async Task<ErrorOr<byte[]>> GetKeyByJwtAsync(string token)
    {
        string keyId = DecodeToken(token).GetValueOrDefault("kid", "");
        if (string.IsNullOrEmpty(keyId))
        {
            return Error.Unauthorized(description: "Token does not contain key id");
        }

        byte[]? publicKey = KeyManager.GetKeyByKeyId(keyId);
        if (publicKey is null)
        {
            publicKey = await _getVerificationKeyService.GetPublicKeyByKeyId(keyId);
            if (publicKey is null)
            {
                return Error.Unauthorized(description: "Could not get public key");
            }

            KeyManager.SaveKey(keyId, publicKey);
        }

        return publicKey;
    }
}
