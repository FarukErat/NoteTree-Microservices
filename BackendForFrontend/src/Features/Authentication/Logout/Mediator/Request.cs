using MediatR;

namespace Features.Authentication.Logout;

public sealed record class LogoutRequest(
    string? SessionId)
    : IRequest<LogoutResponse>;
