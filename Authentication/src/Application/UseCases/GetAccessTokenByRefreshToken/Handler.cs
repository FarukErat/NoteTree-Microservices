using Application.Interfaces.Infrastructure;
using Application.Interfaces.Persistence;
using Domain.Entities;
using ErrorOr;
using MediatR;

namespace Application.UseCases.GetAccessTokenByRefreshToken;

public sealed class GetAccessTokenByRefreshTokenHandler(
    IJwtVerifier jwtVerifier,
    IJwtGenerator jwtGenerator,
    IUserReadRepository userReadRepository
) : IRequestHandler<GetAccessTokenByRefreshTokenRequest, ErrorOr<GetAccessTokenByRefreshTokenResponse>>
{
    private readonly IJwtVerifier _jwtVerifier = jwtVerifier;
    private readonly IJwtGenerator _jwtGenerator = jwtGenerator;
    private readonly IUserReadRepository _userReadRepository = userReadRepository;

    public async Task<ErrorOr<GetAccessTokenByRefreshTokenResponse>> Handle(GetAccessTokenByRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        ErrorOr<Success> result = _jwtVerifier.VerifyToken(
            token: request.RefreshToken,
            audience: request.ClientIp);
        if (result.IsError)
        {
            return Error.Unauthorized(description: result.FirstError.Description);
        }

        Guid? userId = _jwtVerifier.ExtractUserId(request.RefreshToken);
        if (userId is null)
        {
            return Error.Unauthorized(description: "Invalid token");
        }

        User? user = await _userReadRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return Error.NotFound(description: "User not found");
        }

        string accessToken = _jwtGenerator.GenerateAccessToken(
            user: user,
            audience: request.Audience);

        return new GetAccessTokenByRefreshTokenResponse(
            accessToken);
    }
}
