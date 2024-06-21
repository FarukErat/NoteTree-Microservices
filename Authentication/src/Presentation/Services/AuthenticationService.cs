using Application.UseCases.Login;
using Application.UseCases.Register;
using Application.UseCases.GetCurrentPublicKey;
using Application.UseCases.GetPublicKeyByKeyId;
using Application.UseCases.GetAccessTokenByRefreshToken;

using ErrorOr;
using Grpc.Core;
using MediatR;

namespace Presentation.Services;

public sealed class AuthenticationService(
    ISender sender
) : Proto.Authentication.AuthenticationBase
{
    private readonly ISender _sender = sender;

    public override async Task<Proto.RegisterResponse> Register(Proto.RegisterRequest request, ServerCallContext context)
    {
        RegisterRequest mediatorRequest = new(
            Username: request.Username,
            Password: request.Password,
            Email: request.Email,
            FirstName: request.FirstName,
            LastName: request.LastName
        );
        ErrorOr<RegisterResponse> mediatorResponse = await _sender.Send(mediatorRequest);

        if (mediatorResponse.IsError)
        {
            throw CreateRpcException(mediatorResponse.FirstError);
        }

        return new Proto.RegisterResponse()
        {
            UserId = mediatorResponse.Value.UserId.ToString()
        };
    }

    public override async Task<Proto.LoginResponse> Login(Proto.LoginRequest request, ServerCallContext context)
    {
        // string clientPort = context.GetHttpContext().Connection.RemotePort.ToString();
        string? clientIp = context.GetHttpContext().Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(clientIp))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Client IP address is not provided"));
        }

        LoginRequest mediatorRequest = new(
            Username: request.Username,
            Password: request.Password,
            ClientIp: clientIp
        );
        ErrorOr<LoginResponse> mediatorResponse = await _sender.Send(mediatorRequest);

        if (mediatorResponse.IsError)
        {
            throw CreateRpcException(mediatorResponse.FirstError);
        }

        return new Proto.LoginResponse()
        {
            // TODO: include expiration time
            UserId = mediatorResponse.Value.UserId.ToString(),
            RefreshToken = mediatorResponse.Value.RefreshToken
        };
    }

    public override async Task<Proto.GetCurrentPublicKeyResponse> GetCurrentPublicKey(Proto.GetCurrentPublicKeyRequest request, ServerCallContext context)
    {
        GetCurrentPublicKeyRequest mediatorRequest = new();
        ErrorOr<GetCurrentPublicKeyResponse> mediatorResponse = await _sender.Send(mediatorRequest);

        if (mediatorResponse.IsError)
        {
            throw CreateRpcException(mediatorResponse.FirstError);
        }

        return new Proto.GetCurrentPublicKeyResponse
        {
            KeyId = mediatorResponse.Value.KeyId,
            Key = Google.Protobuf.ByteString.CopyFrom(mediatorResponse.Value.PublicKey)
        };
    }

    public override async Task<Proto.GetPublicKeyByKeyIdResponse> GetPublicKeyByKeyId(Proto.GetPublicKeyByKeyIdRequest request, ServerCallContext context)
    {
        GetPublicKeyByKeyIdRequest mediatorRequest = new(
            KeyId: request.KeyId
        );
        ErrorOr<GetPublicKeyByKeyIdResponse> mediatorResponse = await _sender.Send(mediatorRequest);

        if (mediatorResponse.IsError)
        {
            throw CreateRpcException(mediatorResponse.FirstError);
        }

        return new Proto.GetPublicKeyByKeyIdResponse
        {
            Key = Google.Protobuf.ByteString.CopyFrom(mediatorResponse.Value.PublicKey)
        };
    }

    // TODO: add GetAllPublicKeys endpoint

    public override async Task<Proto.GetAccessTokenByRefreshTokenResponse> GetAccessTokenByRefreshToken(Proto.GetAccessTokenByRefreshTokenRequest request, ServerCallContext context)
    {
        string? clientIp = context.GetHttpContext().Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(clientIp))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Client IP address is not provided"));
        }

        GetAccessTokenByRefreshTokenRequest mediatorRequest = new(
            RefreshToken: request.RefreshToken,
            ClientIp: clientIp,
            Audience: request.Audience
        );
        ErrorOr<GetAccessTokenByRefreshTokenResponse> mediatorResponse = await _sender.Send(mediatorRequest);

        if (mediatorResponse.IsError)
        {
            throw CreateRpcException(mediatorResponse.FirstError);
        }

        return new Proto.GetAccessTokenByRefreshTokenResponse
        {
            AccessToken = mediatorResponse.Value.AccessToken
        };
    }

    private static RpcException CreateRpcException(Error error)
    {
        switch (error.Type)
        {
            case ErrorType.Conflict:
                return new RpcException(new Status(StatusCode.AlreadyExists, error.Description));
            case ErrorType.NotFound:
                return new RpcException(new Status(StatusCode.NotFound, error.Description));
            case ErrorType.Validation:
                return new RpcException(new Status(StatusCode.InvalidArgument, error.Description));
            case ErrorType.Unauthorized:
                return new RpcException(new Status(StatusCode.Unauthenticated, error.Description));
            case ErrorType.Forbidden:
                return new RpcException(new Status(StatusCode.PermissionDenied, error.Description));
            case ErrorType.Failure:
            case ErrorType.Unexpected:
            default:
                return new RpcException(new Status(StatusCode.Internal, error.Description));
        }
    }
}
