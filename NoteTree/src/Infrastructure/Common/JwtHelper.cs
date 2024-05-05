using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces.Infrastructure;
using Domain.Enums;
using ErrorOr;
using Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Infrastructure.Common;

public sealed class JwtHelper(
    GetPublicKeyService getVerificationKeyService
) : IJwtHelper
{
    private readonly GetPublicKeyService _getVerificationKeyService = getVerificationKeyService;

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

    public Dictionary<string, dynamic> DecodeToken(string? token)
    {
        if (token is null)
        {
            return [];
        }

        string[] tokenParts = token.Split('.');
        if (tokenParts.Length != 3)
        {
            return [];
        }

        string header = tokenParts[0];
        byte[] headerBytes = Convert.FromBase64String(header);
        string headerJson = Encoding.UTF8.GetString(headerBytes);
        Dictionary<string, dynamic> headerMap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(headerJson) ?? [];

        string payload = tokenParts[1];
        byte[] payloadBytes = Convert.FromBase64String(payload);
        string payloadJson = Encoding.UTF8.GetString(payloadBytes);
        Dictionary<string, dynamic> payloadMap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(payloadJson) ?? [];

        return headerMap.Concat(payloadMap).ToDictionary(pair => pair.Key, pair => pair.Value);
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

    public List<Role> GetUserRoles(string? token)
    {
        dynamic? roles = DecodeToken(token).GetValueOrDefault("role");
        if (roles is null)
        {
            return [];
        }

        List<Role> userRoles = [];
        foreach (string role in roles)
        {
            if (Enum.TryParse(role, out Role userRole))
            {
                userRoles.Add(userRole);
            }
        }

        return userRoles;
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
