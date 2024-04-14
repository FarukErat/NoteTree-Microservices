namespace Presentation.Services;

using Grpc.Core;
using MediatR;

using Application.Mediator.GetNotes;
using Application.Mediator.SetNotes;
using Domain.Models;

public sealed class NoteTreeService(
    ISender sender)
    : NoteTree.NoteTreeBase
{
    private readonly ISender _sender = sender;

    public override async Task<Presentation.GetNotesResponse> GetNotes(Presentation.GetNotesRequest request, ServerCallContext context)
    {
        GetNotesResponse mediatorResponse = await _sender.Send(
            new GetNotesRequest(Guid.Parse(request.Id)));

        Presentation.GetNotesResponse response = new();
        if (mediatorResponse.Notes is null)
        {
            return response;
        }
        response.Notes.AddRange(mediatorResponse.Notes.Select(ConvertNoteToPresentationNote));
        return response;
    }

    public override async Task<Presentation.SetNotesResponse> SetNotes(Presentation.SetNotesRequest request, ServerCallContext context)
    {
        Note[] notes = request.Notes.Select(ConvertPresentationNoteToNote).ToArray();

        Guid id = Guid.TryParse(request.Id, out Guid result) ? result : Guid.Empty;

        await _sender.Send(new SetNotesRequest(id, notes));

        Presentation.SetNotesResponse response = new();
        return response;
    }

    private static Presentation.Note ConvertNoteToPresentationNote(Note note)
    {
        Presentation.Note presentationNote = new()
        {
            Content = note.Content,
        };
        if (note.Children is not null)
        {
            presentationNote.Children.AddRange(note.Children.Select(ConvertNoteToPresentationNote));
        }
        return presentationNote;
    }

    private static Note ConvertPresentationNoteToNote(Presentation.Note presentationNote)
    {
        Note note = new()
        {
            Content = presentationNote.Content,
        };
        if (presentationNote.Children is not null)
        {
            note.Children = presentationNote.Children.Select(ConvertPresentationNoteToNote).ToArray();
        }
        return note;
    }
}
