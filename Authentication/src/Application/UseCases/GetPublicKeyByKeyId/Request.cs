using ErrorOr;
using MediatR;

namespace Application.UseCases.GetPublicKeyByKeyId;

public sealed record class GetPublicKeyByKeyIdRequest(
    string KeyId
) : IRequest<ErrorOr<GetPublicKeyByKeyIdResponse>>;
