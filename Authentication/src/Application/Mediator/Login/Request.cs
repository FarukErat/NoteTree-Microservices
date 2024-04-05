using ErrorOr;
using MediatR;

namespace Application.Mediator.Login;

public sealed record class LoginRequest(
    string Username,
    string Password,
    string? ClientIp = null)
    : IRequest<ErrorOr<LoginResponse>>;
