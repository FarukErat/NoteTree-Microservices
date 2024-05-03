using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using MediatR;

namespace Application.UseCases.GetVerificationKey;

public sealed class GetVerificationKeyHandler(
    IPublicKeyProvider publicKeyProvider
) : IRequestHandler<GetVerificationKeyRequest, GetVerificationKeyResponse>
{
    private readonly IPublicKeyProvider _publicKeyProvider = publicKeyProvider;
    public Task<GetVerificationKeyResponse> Handle(GetVerificationKeyRequest request, CancellationToken cancellationToken)
    {
        string? keyId;
        byte[]? publicKey;
        if (string.IsNullOrEmpty(request.KeyId))
        {
            (keyId, publicKey) = _publicKeyProvider.GetCurrentPublicKey();
        }
        else
        {
            (keyId, publicKey) = _publicKeyProvider.GetPublicKeyById(request.KeyId);
        }
        return Task.FromResult(new GetVerificationKeyResponse(
            KeyId: keyId,
            VerificationKey: publicKey));
    }
}
