namespace Presentation.Services; // at the top of the file to avoid type conflicts with protobuf-generated code

using Application.Features.GetVerificationKey;
using Application.Features.Register;
using Application.Features.Login;

using ErrorOr;
using Grpc.Core;
using MediatR;

public sealed class AuthenticationService(
    ISender sender
) : Authentication.AuthenticationBase
{
    private readonly ISender _sender = sender;

    public override async Task<Presentation.RegisterResponse> Register(Presentation.RegisterRequest request, ServerCallContext context)
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
            return new Presentation.RegisterResponse()
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

    public override async Task<Presentation.LoginResponse> Login(Presentation.LoginRequest request, ServerCallContext context)
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
            return new Presentation.LoginResponse()
            {
                UserId = mediatorResponse.Value.UserId.ToString(),
                Token = mediatorResponse.Value.Token
            };
        }

        throw mediatorResponse.FirstError.Type switch
        {
            ErrorType.Conflict => new RpcException(new Status(StatusCode.PermissionDenied, mediatorResponse.FirstError.Description)),
            ErrorType.NotFound => new RpcException(new Status(StatusCode.NotFound, mediatorResponse.FirstError.Description)),
            ErrorType.Validation => new RpcException(new Status(StatusCode.InvalidArgument, mediatorResponse.FirstError.Description)),
            _ => new RpcException(new Status(StatusCode.Internal, mediatorResponse.FirstError.Description)),
        };
    }

    public override async Task<Presentation.VerificationKeyResponse> GetVerificationKey(Presentation.VerificationKeyRequest request, ServerCallContext context)
    {
        GetVerificationKeyRequest mediatorRequest = new();
        GetVerificationKeyResponse mediatorResponse = await _sender.Send(mediatorRequest);

        return new VerificationKeyResponse
        {
            Key = mediatorResponse.VerificationKey
        };
    }
}
