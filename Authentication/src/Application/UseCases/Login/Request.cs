using ErrorOr;
using MediatR;

namespace Application.UseCases.Login;

public sealed record class LoginRequest(
    string Username,
    string Password,
    string? ClientIp = null
) : IRequest<ErrorOr<LoginResponse>>;
