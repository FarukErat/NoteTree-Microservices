using Domain.Models;

namespace Application.UseCases.GetNotes;

public sealed record class GetNotesResponse(
    Note[] Notes
);
