namespace Features.Authentication.Register;

public sealed record class RegisterRequestDto(
    string Username,
    string Password,
    string Email,
    string FirstName,
    string LastName);
