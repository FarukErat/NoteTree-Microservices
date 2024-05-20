using Application.Interfaces.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using Domain.Entities;
using Domain.Enums;
using System.Text.Json;

namespace Infrastructure.Common;

public sealed class JwtGenerator(
    string keyId,
    byte[] privateKey
) : IJwtGenerator
{
    private readonly string _keyId = keyId;
    private readonly byte[] _privateKey = privateKey;

    public string GenerateRefreshToken(Guid userId, string audience)
    {
        RSA rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(_privateKey, out _);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = Configurations.Jwt.Issuer,
            Subject = new ClaimsIdentity([new Claim("sub", userId.ToString())]),
            Audience = audience,
            Expires = DateTime.UtcNow.Add(Configurations.Jwt.RefreshTokenExpiry),
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        };

        JwtSecurityTokenHandler tokenHandler = new();
        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        token.Header.Add("kid", _keyId);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateAccessToken(User user, string audience)
    {
        RSA rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(_privateKey, out _);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = Configurations.Jwt.Issuer,
            Subject = new ClaimsIdentity([
                new Claim("sub", user.Id.ToString()),
                new Claim("jti", new Guid().ToString()),
                new Claim("roles", user.Roles.ToRolesJson(), JsonClaimValueTypes.JsonArray),
            ]),
            Audience = audience,
            Expires = DateTime.UtcNow.Add(Configurations.Jwt.AccessTokenExpiry),
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        };

        JwtSecurityTokenHandler tokenHandler = new();
        JwtSecurityToken token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        token.Header.Add("kid", _keyId);

        return tokenHandler.WriteToken(token);
    }
}

public static class UIntExtensions
{
    public static string ToRolesJson(this uint number)
    {
        List<string> roles = [];

        int size = sizeof(uint) * 8;

        for (int i = 0; i < size; i++)
        {
            if ((number & (1 << i)) != 0)
            {
                roles.Add(((Role)i).ToString());
            }
        }

        return JsonSerializer.Serialize(roles);
    }
}
