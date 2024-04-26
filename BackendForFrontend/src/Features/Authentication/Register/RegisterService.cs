using ErrorOr;
using Grpc.Core;
using Grpc.Net.Client;

using Infrastructure;
using static Features.Authentication.Register.Proto.Authentication;

namespace Features.Authentication.Register;

public sealed class RegisterService
{
    private readonly GrpcChannel _channel;
    private readonly AuthenticationClient _client;

    public RegisterService()
    {
        _channel = GrpcChannel.ForAddress(Configurations.AuthenticationUrl);
        _client = new AuthenticationClient(_channel);
    }

    public async Task<ErrorOr<Guid>> Register(
        string username,
        string password,
        string email,
        string firstName,
        string lastName)
    {
        Proto.RegisterRequest registerRequest = new()
        {
            Username = username,
            Password = password,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        try
        {
            Proto.RegisterResponse response = await _client.RegisterAsync(registerRequest);
            return Guid.Parse(response.UserId);
        }
        catch (RpcException e)
        {
            return e.StatusCode switch
            {
                StatusCode.InvalidArgument => Error.Validation(description: e.Message),
                StatusCode.AlreadyExists => Error.Conflict(description: e.Message),
                _ => Error.Unexpected(description: e.Message),
            };
        }
    }
}
