using Application.Interfaces.Persistence;
using MediatR;

namespace Application.Mediator.Logout;

public sealed class LogoutHandler(
    ICacheService cacheService)
    : IRequestHandler<LogoutRequest, LogoutResponse>
{
    private readonly ICacheService _cacheService = cacheService;
    public Task<LogoutResponse> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        _cacheService.DeleteSessionByIdAsync(request.SessionId);
        return Task.FromResult(new LogoutResponse());
    }
}
