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
    public Role Role { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public PasswordHashAlgorithm PasswordHashAlgorithm { get; set; }

    // foreign keys
    public Guid NoteRecordId { get; set; }
}
