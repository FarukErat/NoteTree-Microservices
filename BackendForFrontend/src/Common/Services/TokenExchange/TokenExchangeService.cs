using ErrorOr;
using Grpc.Core;
using Grpc.Net.Client;

using Infrastructure;
using static Common.Services.TokenExchangeService.Proto.Authentication;

namespace Common.Services.TokenExchangeService;

public sealed class TokenExchangeService
{
    private readonly GrpcChannel _channel;
    private readonly AuthenticationClient _client;

    public TokenExchangeService()
    {
        _channel = GrpcChannel.ForAddress(Configurations.ConnectionStrings.AuthenticationServiceUrl);
        _client = new AuthenticationClient(_channel);
    }

    public async Task<string?> GetAccessTokenByRefreshToken(string refreshToken, string audience)
    {
        Proto.GetAccessTokenByRefreshTokenRequest tokenExchangeRequest = new()
        {
            RefreshToken = refreshToken,
            Audience = audience
        };

        try
        {
            Proto.GetAccessTokenByRefreshTokenResponse response = await _client.GetAccessTokenByRefreshTokenAsync(tokenExchangeRequest);
            return response.AccessToken;
        }
        catch
        {
            return null;
        }
    }
}
