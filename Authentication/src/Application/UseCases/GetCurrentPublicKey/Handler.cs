using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using MediatR;

namespace Application.UseCases.GetCurrentPublicKey;

public sealed class GetCurrentPublicKeyHandler(
    IPublicKeyProvider publicKeyProvider
) : IRequestHandler<GetCurrentPublicKeyRequest, GetCurrentPublicKeyResponse>
{
    private readonly IPublicKeyProvider _publicKeyProvider = publicKeyProvider;
    public Task<GetCurrentPublicKeyResponse> Handle(GetCurrentPublicKeyRequest request, CancellationToken cancellationToken)
    {
        (string? keyId, byte[]? publicKey) = _publicKeyProvider.GetCurrentPublicKey();
        return Task.FromResult(new GetCurrentPublicKeyResponse(
            KeyId: keyId,
            PublicKey: publicKey));
    }
}
