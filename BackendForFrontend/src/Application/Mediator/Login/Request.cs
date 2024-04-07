using ErrorOr;
using MediatR;

namespace Application.Mediator.Login;

public sealed record class LoginRequest(
    string Username,
    string Password,
    string UserAgent,
    string? IpAddress = null)
    : IRequest<ErrorOr<LoginResponse>>;
