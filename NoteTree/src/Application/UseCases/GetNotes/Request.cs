using ErrorOr;
using MediatR;
using Security;

namespace Application.UseCases.GetNotes;

public sealed record class GetNotesRequest(
    string Jwt
) : IAuthorizeableRequest<ErrorOr<GetNotesResponse>>;
