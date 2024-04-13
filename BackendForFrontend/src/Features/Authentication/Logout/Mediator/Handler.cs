using Common.Interfaces;
using MediatR;

namespace Features.Authentication.Logout;

public sealed class LogoutHandler(
    ICacheService cacheService)
    : IRequestHandler<LogoutRequest, LogoutResponse>
{
    private readonly ICacheService _cacheService = cacheService;
    public async Task<LogoutResponse> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        if (request.SessionId is not null)
        {
            await _cacheService.DeleteSessionByIdAsync(request.SessionId);
        }

        return new LogoutResponse();
    }
}
