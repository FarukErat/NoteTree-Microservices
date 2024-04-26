using Domain.Models;
using MediatR;
using ErrorOr;
using Security;
using Application.Security;
using Domain.Enums;

namespace Application.UseCases.SetNotes;

[Authorize(Roles = [Role.Admin, Role.User])]
public sealed record class SetNotesRequest(
    string Jwt,
    Note[] Notes
) : IAuthorizeableRequest<ErrorOr<SetNotesResponse>>;
