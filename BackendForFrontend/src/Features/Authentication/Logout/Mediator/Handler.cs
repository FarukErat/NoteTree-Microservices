using Infrastructure.Common;
using MediatR;

namespace Features.Authentication.Logout;

public sealed class LogoutHandler(
    CacheService cacheService
) : IRequestHandler<LogoutRequest, LogoutResponse>
{
    private readonly CacheService _cacheService = cacheService;
    public async Task<LogoutResponse> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        await _cacheService.DeleteSessionByIdAsync(request.SessionId);

        return new LogoutResponse();
    }
}
