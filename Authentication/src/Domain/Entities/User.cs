using Domain.Enums;

namespace Domain.Entities;

// TODO: add business logic with extension methods to keep the entity class clean

public sealed class User : BaseEntity
{
    // properties
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public uint Roles { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public PasswordHashAlgorithm PasswordHashAlgorithm { get; set; }
}
