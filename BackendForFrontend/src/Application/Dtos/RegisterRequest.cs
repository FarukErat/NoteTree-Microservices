namespace Application.Dtos;

public sealed record class RegisterRequest(
    string Username,
    string Password,
    string Email,
    string FirstName,
    string LastName);
