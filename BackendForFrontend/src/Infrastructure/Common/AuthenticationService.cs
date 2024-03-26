using Grpc.Net.Client;
using Application.Interfaces.Infrastructure;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using static Infrastructure.Authentication;

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
        RegisterResponse reply = client.Register(request);
        if (reply.Success)
        {
            return Result.Success;
        }
        return Error.Failure(reply.Message);
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
        LoginResponse reply = client.Login(request);
        if (reply.Success)
        {
            return reply.Token;
        }
        return Error.Failure(reply.Message);
    }

    public ErrorOr<Success> Logout(HttpContext httpContext)
    {
        httpContext.Response.Cookies.Delete("access_token");
        return Result.Success;
    }
}
