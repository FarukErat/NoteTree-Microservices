using Common.Interfaces;
using Common.Models;
using ErrorOr;
using Infrastructure;
using MediatR;

namespace Features.Authentication.Login;

public sealed class LoginHandler(
    LoginService client,
    ICacheService cacheService)
    : IRequestHandler<LoginRequest, ErrorOr<LoginResponse>>
{
    private readonly LoginService _client = client;
    private readonly ICacheService _cacheService = cacheService;
    public async Task<ErrorOr<LoginResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        ErrorOr<(Guid UserId, string Token)> result = await _client.Login(request.Username, request.Password);

        if (result.IsError)
        {
            return result.FirstError;
        }

        Guid UserId = result.Value.UserId;
        string Token = result.Value.Token;

        Session newSession = new()
        {
            UserId = UserId,
            Token = Token,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow + Configurations.SessionDuration
        };

        Guid sessionId;
        Session? existingSession = await _cacheService.GetSessionByUserIdAsync(UserId);
        if (existingSession is not null)
        {
            // TODO: check for user agent to allow multiple sessions
            // session.UserAgent != httpContext.Request.Headers.UserAgent
            await _cacheService.UpdateSessionByIdAsync(existingSession.Id, newSession);
            sessionId = existingSession.Id;
        }
        else
        {
            sessionId = await _cacheService.SaveSessionAsync(newSession);
        }

        return new LoginResponse(
            UserId: UserId,
            Token: Token,
            SessionId: sessionId);
    }
}
