using MediatR;

namespace Application.UseCases.GetCurrentPublicKey;

public sealed record class GetCurrentPublicKeyRequest
    : IRequest<GetCurrentPublicKeyResponse>;
