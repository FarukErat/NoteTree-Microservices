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
    byte[] privateKey,
    string issuer,
    TimeSpan expiry
) : IJwtGenerator
{
    private readonly string _keyId = keyId;
    private readonly byte[] _privateKey = privateKey;
    private readonly string _issuer = issuer;
    private readonly TimeSpan _expiry = expiry;

    public string GenerateToken(User user, string audience)
    {
        RSA rsa = RSA.Create();
        rsa.ImportRSAPrivateKey(_privateKey, out _);

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Issuer = _issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Roles.ToRolesJson(), JsonClaimValueTypes.JsonArray),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            ]),
            Expires = DateTime.UtcNow.Add(_expiry),
            SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        };

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

        for (int i = 0; i < 32; i++)
        {
            if ((number & (1 << i)) != 0)
            {
                roles.Add(((Role)i).ToString());
            }
        }

        return JsonSerializer.Serialize(roles);
    }
}
