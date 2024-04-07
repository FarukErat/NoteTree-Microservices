namespace Presentation.DTOs.Register;

public sealed record class RegisterRequest(
    string Username,
    string Password,
    string Email,
    string FirstName,
    string LastName);
