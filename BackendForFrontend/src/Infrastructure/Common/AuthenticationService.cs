using Grpc.Net.Client;
using Application.Interfaces.Infrastructure;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using static Infrastructure.Authentication;
using Grpc.Core;
using Application.Interfaces.Persistence;
using Application.Models;

namespace Infrastructure.Common;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly GrpcChannel _channel;
    private readonly AuthenticationClient _client;
    private readonly ICacheService _cacheService;
    private readonly string _sessionIdKey = "SID";

    public AuthenticationService(ICacheService cacheService)
    {
        _cacheService = cacheService;
        _channel = GrpcChannel.ForAddress(Configurations.AuthenticationUrl);
        _client = new AuthenticationClient(_channel);
    }

    public ErrorOr<Application.Dtos.RegisterResponse> Register(Application.Dtos.RegisterRequest registerRequest)
    {
        RegisterRequest request = new()
        {
            Username = registerRequest.Username,
            Password = registerRequest.Password,
            Email = registerRequest.Email,
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName
        };

        try
        {
            RegisterResponse reply = _client.Register(request);
            return new Application.Dtos.RegisterResponse(
                UserId: Guid.Parse(reply.UserId));
        }
        catch (RpcException e)
        {
            return e.StatusCode switch
            {
                StatusCode.InvalidArgument => Error.Validation(description: e.Message),
                StatusCode.AlreadyExists => Error.Conflict(description: e.Message),
                _ => Error.Failure("An error occurred"),
            };
        }
    }

    public async Task<ErrorOr<Application.Dtos.LoginResponse>> Login(Application.Dtos.LoginRequest loginRequest, HttpContext httpContext)
    {
        LoginRequest request = new()
        {
            Username = loginRequest.Username,
            Password = loginRequest.Password
        };
        try
        {
            LoginResponse reply = _client.Login(request);

            string sessionId = await _cacheService.SaveSessionAsync(new Session(
                UserId: reply.UserId,
                Token: reply.Token,
                IpAddress: httpContext.Connection.RemoteIpAddress?.ToString() ?? "",
                UserAgent: httpContext.Request.Headers.UserAgent.ToString(),
                CreatedAt: DateTime.UtcNow,
                ExpireAt: DateTime.UtcNow.AddMinutes(30)
            ));
            httpContext.Response.Cookies.Append(_sessionIdKey, sessionId);

            return new Application.Dtos.LoginResponse(
                UserId: Guid.Parse(reply.UserId),
                Token: reply.Token);
        }
        catch (RpcException e)
        {
            return e.StatusCode switch
            {
                StatusCode.InvalidArgument => Error.Validation(description: e.Message),
                StatusCode.NotFound => Error.NotFound(e.Message),
                StatusCode.Unauthenticated => Error.Unauthorized(description: e.Message),
                _ => Error.Failure("An error occurred"),
            };
        }
    }

    public ErrorOr<Success> Logout(HttpContext httpContext)
    {
        string? sessionId = httpContext.Request.Cookies[_sessionIdKey];
        if (sessionId is not null)
        {
            _cacheService.DeleteSessionByIdAsync(sessionId);
            httpContext.Response.Cookies.Delete(_sessionIdKey);
        }
        return Result.Success;
    }
}
