using MediatR;

namespace Application.UseCases.GetVerificationKey;

public sealed record class GetVerificationKeyRequest
    : IRequest<GetVerificationKeyResponse>;
