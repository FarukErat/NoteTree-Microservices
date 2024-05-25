using Domain.Enums;

namespace Domain.Entities;

public sealed class User
{
    // primary key
    public Guid Id { get; set; }

    // properties
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public uint Roles { get; set; }
    // TODO: consider embedding the password hash algorithm and its parameters in the password hash
    public string PasswordHash { get; set; } = string.Empty;
    public PasswordHashAlgorithm PasswordHashAlgorithm { get; set; }
}
