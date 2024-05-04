using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using MediatR;

namespace Application.UseCases.GetPublicKeyByKeyId;

public sealed class GetPublicKeyByKeyIdHandler(
    IPublicKeyProvider publicKeyProvider
) : IRequestHandler<GetPublicKeyByKeyIdRequest, GetPublicKeyByKeyIdResponse>
{
    private readonly IPublicKeyProvider _publicKeyProvider = publicKeyProvider;
    public Task<GetPublicKeyByKeyIdResponse> Handle(GetPublicKeyByKeyIdRequest request, CancellationToken cancellationToken)
    {
        byte[]? publicKey = _publicKeyProvider.GetPublicKeyById(request.KeyId);
        return Task.FromResult(new GetPublicKeyByKeyIdResponse(
            PublicKey: publicKey));
    }
}
