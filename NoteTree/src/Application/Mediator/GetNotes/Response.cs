using Domain.Models;

namespace Application.Mediator.GetNotes;

public sealed record class GetNotesResponse(
    Note[]? Notes);
