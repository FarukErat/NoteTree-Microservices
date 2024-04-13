using ErrorOr;
using MediatR;

namespace Features.Authentication.Register;

public sealed record class RegisterRequest(
    string Username,
    string Password,
    string Email,
    string FirstName,
    string LastName)
    : IRequest<ErrorOr<RegisterResponse>>;
