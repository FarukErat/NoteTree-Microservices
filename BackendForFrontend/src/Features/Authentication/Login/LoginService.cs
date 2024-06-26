using ErrorOr;
using Grpc.Core;
using Grpc.Net.Client;

using Infrastructure;
using static Features.Authentication.Login.Proto.Authentication;

namespace Features.Authentication.Login;

public sealed class LoginService
{
    private readonly GrpcChannel _channel;
    private readonly AuthenticationClient _client;

    public LoginService()
    {
        _channel = GrpcChannel.ForAddress(Configurations.ConnectionStrings.AuthenticationServiceUrl);
        _client = new AuthenticationClient(_channel);
    }

    public async Task<ErrorOr<(Guid UserId, string Token)>> Login(string username, string password)
    {
        Proto.LoginRequest loginRequest = new()
        {
            Username = username,
            Password = password
        };

        try
        {
            Proto.LoginResponse response = await _client.LoginAsync(loginRequest);
            return (Guid.Parse(response.UserId), response.RefreshToken);
        }
        catch (RpcException e)
        {
            return e.StatusCode switch
            {
                StatusCode.InvalidArgument => Error.Validation(description: e.Message),
                StatusCode.NotFound => Error.NotFound(description: e.Message),
                StatusCode.PermissionDenied => Error.Conflict(description: e.Message),
                _ => Error.Unexpected(description: e.Message),
            };
        }
    }
}
