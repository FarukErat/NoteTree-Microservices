using ErrorOr;
using MediatR;

namespace Application.UseCases.Register;

public sealed record class RegisterRequest(
    string Username,
    string Password,
    string Email,
    string FirstName,
    string LastName
) : IRequest<ErrorOr<RegisterResponse>>;
