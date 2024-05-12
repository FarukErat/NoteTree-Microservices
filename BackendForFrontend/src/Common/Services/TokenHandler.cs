using Infrastructure.Common;

namespace Common.Services;

public sealed class TokenHandler(
    CacheService cacheService,
    TokenExchangeService.TokenExchangeService tokenExchangeService
)
{
    private readonly CacheService _cacheService = cacheService;
    private readonly TokenExchangeService.TokenExchangeService _tokenExchangeService = tokenExchangeService;

    public async Task<string?> GetAccessTokenBySessionIdAsync(Guid sessionId, string audience)
    {
        string? refreshToken = await _cacheService.GetTokenByIdAsync(sessionId);
        if (refreshToken is null)
        {
            return null;
        }

        string? accessToken = await _tokenExchangeService.GetAccessTokenByRefreshToken(refreshToken, audience);

        return accessToken;
    }
}
