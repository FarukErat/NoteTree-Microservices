using ErrorOr;
using Grpc.Core;
using Grpc.Net.Client;
using Application.Interfaces.Infrastructure;
using static Infrastructure.Authentication;

namespace Infrastructure.Common;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly GrpcChannel _channel;
    private readonly AuthenticationClient _client;

    public AuthenticationService()
    {
        _channel = GrpcChannel.ForAddress(Configurations.AuthenticationUrl);
        _client = new AuthenticationClient(_channel);
    }

    public async Task<ErrorOr<(Guid UserId, string Token)>> Login(string username, string password)
    {
        await Task.CompletedTask;
        LoginRequest loginRequest = new()
        {
            Username = username,
            Password = password
        };

        try
        {
            LoginResponse response = _client.Login(loginRequest);
            return (Guid.Parse(response.UserId), response.Token);
        }
        catch (RpcException e)
        {
            return e.StatusCode switch
            {
                StatusCode.InvalidArgument => Error.Validation(e.Message),
                StatusCode.NotFound => Error.NotFound(e.Message),
                _ => Error.Unexpected(e.Message),
            };
        }
    }

    public async Task<ErrorOr<Guid>> Register(
        string username,
        string password,
        string email,
        string firstName,
        string lastName)
    {
        await Task.CompletedTask;
        RegisterRequest registerRequest = new()
        {
            Username = username,
            Password = password,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        try
        {
            RegisterResponse response = _client.Register(registerRequest);
            return Guid.Parse(response.UserId);
        }
        catch (RpcException e)
        {
            return e.StatusCode switch
            {
                StatusCode.InvalidArgument => Error.Validation(e.Message),
                StatusCode.AlreadyExists => Error.Conflict(e.Message),
                _ => Error.Unexpected(e.Message),
            };
        }
    }
}
