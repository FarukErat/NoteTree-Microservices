using Application.Interfaces.Infrastructure;
using MediatR;

namespace Application.Mediator.GetVerificationKey;

public sealed class GetVerificationKeyHandler(
    IRsaKeyPair rsaKeyPair
) : IRequestHandler<GetVerificationKeyRequest, GetVerificationKeyResponse>
{
    private readonly IRsaKeyPair _rsaKeyPair = rsaKeyPair;
    public Task<GetVerificationKeyResponse> Handle(GetVerificationKeyRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetVerificationKeyResponse(
            VerificationKey: _rsaKeyPair.PublicKey));
    }
}
