using MediatR;

namespace Application.Mediator.GetVerificationKey;

public sealed record class GetVerificationKeyRequest
    : IRequest<GetVerificationKeyResponse>;
