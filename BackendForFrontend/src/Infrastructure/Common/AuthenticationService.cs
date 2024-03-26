using Grpc.Net.Client;
using Application.Interfaces.Infrastructure;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using static Infrastructure.Authentication;
using Grpc.Core;

namespace Infrastructure.Common;

public sealed class AuthenticationService : IAuthenticationService
{
    public ErrorOr<Success> Register(Application.Dtos.RegisterRequest registerRequest)
    {
        using GrpcChannel channel = GrpcChannel.ForAddress(Configurations.AuthenticationUrl);
        AuthenticationClient client = new(channel);
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
            RegisterResponse reply = client.Register(request);
            return Result.Success;
        }
        catch (RpcException e)
        {
            return e.StatusCode switch
            {
                StatusCode.InvalidArgument => Error.Failure(e.Message),
                StatusCode.AlreadyExists => Error.Conflict(e.Message),
                _ => Error.Failure("An error occurred"),
            };
        }
    }

    public ErrorOr<string> Login(Application.Dtos.LoginRequest loginRequest)
    {
        using GrpcChannel channel = GrpcChannel.ForAddress(Configurations.AuthenticationUrl);
        AuthenticationClient client = new(channel);
        LoginRequest request = new()
        {
            Username = loginRequest.Username,
            Password = loginRequest.Password
        };
        try
        {
            LoginResponse reply = client.Login(request);
            return reply.Token;
        }
        catch (RpcException e)
        {
            return e.StatusCode switch
            {
                StatusCode.InvalidArgument => Error.Failure(e.Message),
                StatusCode.NotFound => Error.NotFound(e.Message),
                StatusCode.Unauthenticated => Error.Unauthorized(e.Message),
                _ => Error.Failure("An error occurred"),
            };
        }
    }

    public ErrorOr<Success> Logout(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("access_token");
        return Result.Success;
    }
}
