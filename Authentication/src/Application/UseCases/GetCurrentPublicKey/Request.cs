using ErrorOr;
using MediatR;

namespace Application.UseCases.GetCurrentPublicKey;

public sealed record class GetCurrentPublicKeyRequest
    : IRequest<ErrorOr<GetCurrentPublicKeyResponse>>;
