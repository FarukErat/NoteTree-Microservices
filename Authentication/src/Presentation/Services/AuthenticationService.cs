using Application.UseCases.GetCurrentPublicKey;
using Application.UseCases.GetPublicKeyByKeyId;
using Application.UseCases.Register;
using Application.UseCases.Login;

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

        if (!mediatorResponse.IsError)
        {
            return new Proto.RegisterResponse()
            {
                UserId = mediatorResponse.Value.UserId.ToString()
            };
        }

        throw mediatorResponse.FirstError.Type switch
        {
            ErrorType.Conflict => new RpcException(new Status(StatusCode.AlreadyExists, mediatorResponse.FirstError.Description)),
            ErrorType.Validation => new RpcException(new Status(StatusCode.InvalidArgument, mediatorResponse.FirstError.Description)),
            _ => new RpcException(new Status(StatusCode.Internal, mediatorResponse.FirstError.Description)),
        };
    }

    public override async Task<Proto.LoginResponse> Login(Proto.LoginRequest request, ServerCallContext context)
    {
        LoginRequest mediatorRequest = new(
            Username: request.Username,
            Password: request.Password,
            ClientIp: context.GetHttpContext().Connection.RemoteIpAddress?.ToString()
        );
        // string clientPort = context.GetHttpContext().Connection.RemotePort.ToString();
        ErrorOr<LoginResponse> mediatorResponse = await _sender.Send(mediatorRequest);

        if (!mediatorResponse.IsError)
        {
            return new Proto.LoginResponse()
            {
                UserId = mediatorResponse.Value.UserId.ToString(),
                Token = mediatorResponse.Value.Token
            };
        }

        throw mediatorResponse.FirstError.Type switch
        {
            ErrorType.Conflict => new RpcException(new Status(StatusCode.AlreadyExists, mediatorResponse.FirstError.Description)),
            ErrorType.NotFound => new RpcException(new Status(StatusCode.NotFound, mediatorResponse.FirstError.Description)),
            ErrorType.Validation => new RpcException(new Status(StatusCode.InvalidArgument, mediatorResponse.FirstError.Description)),
            _ => new RpcException(new Status(StatusCode.Internal, mediatorResponse.FirstError.Description)),
        };
    }

    public override async Task<Proto.GetCurrentPublicKeyResponse> GetCurrentPublicKey(Proto.GetCurrentPublicKeyRequest request, ServerCallContext context)
    {
        GetCurrentPublicKeyRequest mediatorRequest = new();
        GetCurrentPublicKeyResponse mediatorResponse = await _sender.Send(mediatorRequest);

        if (mediatorResponse.KeyId is null || mediatorResponse.PublicKey is null)
        {
            throw new RpcException(new Status(StatusCode.Internal, "Public key not found"));
        }

        return new Proto.GetCurrentPublicKeyResponse
        {
            KeyId = mediatorResponse.KeyId,
            Key = Google.Protobuf.ByteString.CopyFrom(mediatorResponse.PublicKey)
        };
    }

    public override async Task<Proto.GetPublicKeyByKeyIdResponse> GetPublicKeyByKeyId(Proto.GetPublicKeyByKeyIdRequest request, ServerCallContext context)
    {
        GetPublicKeyByKeyIdRequest mediatorRequest = new(
            KeyId: request.KeyId
        );
        GetPublicKeyByKeyIdResponse mediatorResponse = await _sender.Send(mediatorRequest);

        if (mediatorResponse.PublicKey is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Public key not found"));
        }

        return new Proto.GetPublicKeyByKeyIdResponse
        {
            Key = Google.Protobuf.ByteString.CopyFrom(mediatorResponse.PublicKey)
        };
    }
}
