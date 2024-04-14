using MediatR;

namespace Features.Authentication.Logout;

public sealed record class LogoutRequest(
    Guid SessionId)
    : IRequest<LogoutResponse>;
