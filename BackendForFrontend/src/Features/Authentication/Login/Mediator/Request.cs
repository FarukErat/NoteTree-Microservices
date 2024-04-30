using ErrorOr;
using MediatR;

namespace Features.Authentication.Login;

public sealed record class LoginRequest(
    string Username,
    string Password,
    string UserAgent,
    string? IpAddress = null
) : IRequest<ErrorOr<LoginResponse>>;
