using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using ErrorOr;
using MediatR;

namespace Application.UseCases.GetPublicKeyByKeyId;

public sealed class GetPublicKeyByKeyIdHandler(
    IPublicKeyProvider publicKeyProvider
) : IRequestHandler<GetPublicKeyByKeyIdRequest, ErrorOr<GetPublicKeyByKeyIdResponse>>
{
    private readonly IPublicKeyProvider _publicKeyProvider = publicKeyProvider;
    public Task<ErrorOr<GetPublicKeyByKeyIdResponse>> Handle(GetPublicKeyByKeyIdRequest request, CancellationToken cancellationToken)
    {
        byte[]? publicKey = _publicKeyProvider.GetPublicKeyById(request.KeyId);
        if (publicKey is null)
        {
            return Task.FromResult<ErrorOr<GetPublicKeyByKeyIdResponse>>(Error.NotFound(description: "Public key not found"));
        }
        return Task.FromResult<ErrorOr<GetPublicKeyByKeyIdResponse>>(new GetPublicKeyByKeyIdResponse(
            PublicKey: publicKey));
    }
}
