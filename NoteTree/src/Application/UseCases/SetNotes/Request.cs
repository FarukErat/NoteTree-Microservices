using Domain.Models;
using MediatR;
using ErrorOr;
using Security;

namespace Application.UseCases.SetNotes;

public sealed record class SetNotesRequest(
    string Jwt,
    Note[] Notes
) : IAuthorizeableRequest<ErrorOr<SetNotesResponse>>;
