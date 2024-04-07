using MediatR;

namespace Application.Mediator.Logout;

public sealed record class LogoutRequest(
    string SessionId)
    : IRequest<LogoutResponse>;
