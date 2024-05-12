using ErrorOr;
using MediatR;

namespace Application.UseCases.GetAccessTokenByRefreshToken;

public sealed record class GetAccessTokenByRefreshTokenRequest(
    string RefreshToken,
    string ClientIp, // verify audience in RefreshToken with this
    string Audience // audience that the new access token will be valid for
) : IRequest<ErrorOr<GetAccessTokenByRefreshTokenResponse>>;
