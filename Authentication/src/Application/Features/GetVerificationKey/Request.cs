using MediatR;

namespace Application.Features.GetVerificationKey;

public sealed record class GetVerificationKeyRequest
    : IRequest<GetVerificationKeyResponse>;
