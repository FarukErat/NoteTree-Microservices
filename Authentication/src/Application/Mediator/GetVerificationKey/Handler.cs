using Application.Interfaces.Infrastructure;
using MediatR;

namespace Application.Mediator.GetVerificationKey;

public sealed class GetVerificationKeyHandler(
    IRsaKeyPair rsaKeyPair)
    : IRequestHandler<GetVerificationKeyRequest, GetVerificationKeyResponse>
{
    private readonly IRsaKeyPair _rsaKeyPair = rsaKeyPair;
    public async Task<GetVerificationKeyResponse> Handle(GetVerificationKeyRequest request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return new GetVerificationKeyResponse(
            VerificationKey: _rsaKeyPair.PublicKey);
    }
}
