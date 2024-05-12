using Application.Interfaces;
using Application.Interfaces.Infrastructure;
using ErrorOr;
using MediatR;

namespace Application.UseCases.GetCurrentPublicKey;

public sealed class GetCurrentPublicKeyHandler(
    IPublicKeyProvider publicKeyProvider
) : IRequestHandler<GetCurrentPublicKeyRequest, ErrorOr<GetCurrentPublicKeyResponse>>
{
    private readonly IPublicKeyProvider _publicKeyProvider = publicKeyProvider;
    public Task<ErrorOr<GetCurrentPublicKeyResponse>> Handle(GetCurrentPublicKeyRequest request, CancellationToken cancellationToken)
    {
        (string? keyId, byte[]? publicKey) = _publicKeyProvider.GetCurrentPublicKey();
        if (keyId is null || publicKey is null)
        {
            return Task.FromResult<ErrorOr<GetCurrentPublicKeyResponse>>(Error.NotFound(description: "Public key not found"));
        }

        return Task.FromResult<ErrorOr<GetCurrentPublicKeyResponse>>(new GetCurrentPublicKeyResponse(
            KeyId: keyId,
            PublicKey: publicKey));
    }
}
