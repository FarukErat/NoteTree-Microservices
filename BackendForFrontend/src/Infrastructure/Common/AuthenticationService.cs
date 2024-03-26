using Grpc.Net.Client;
using Application.Interfaces.Infrastructure;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using static Infrastructure.Authentication;
using Domain.Entities;

namespace Infrastructure.Common;

public sealed class AuthenticationService : IAuthenticationService
{
    public ErrorOr<Success> Register(User user, string password)
    {
        using GrpcChannel channel = GrpcChannel.ForAddress(Configurations.AuthenticationUrl);
        AuthenticationClient client = new(channel);
        RegisterRequest request = new()
        {
            Username = user.Username,
            Password = password,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
        RegisterResponse reply = client.Register(request);
        if (reply.Success)
        {
            return Result.Success;
        }
        return Error.Failure(reply.Message);
    }

    public ErrorOr<string> Login(string username, string password)
    {
        using GrpcChannel channel = GrpcChannel.ForAddress(Configurations.AuthenticationUrl);
        AuthenticationClient client = new(channel);
        LoginRequest request = new()
        {
            Username = username,
            Password = password
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
