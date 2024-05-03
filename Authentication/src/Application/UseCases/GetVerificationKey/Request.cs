using MediatR;

namespace Application.UseCases.GetVerificationKey;

public sealed record class GetVerificationKeyRequest(
    string KeyId
) : IRequest<GetVerificationKeyResponse>;
