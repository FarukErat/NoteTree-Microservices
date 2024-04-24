using Grpc.Core;
using MediatR;

using Application.UseCases.GetNotes;
using Application.UseCases.SetNotes;

using Domain.Models;
using ErrorOr;

namespace Presentation.Services;

public sealed class NoteTreeService(
    ISender sender
) : Proto.NoteTree.NoteTreeBase
{
    private readonly ISender _sender = sender;

    public override async Task<Proto.GetNotesResponse> GetNotes(Proto.GetNotesRequest request, ServerCallContext context)
    {
        Guid id = Guid.TryParse(request.Id, out Guid result) ? result : Guid.Empty;
        if (id == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID"));
        }

        GetNotesResponse mediatorResponse = await _sender.Send(
            new GetNotesRequest(id));

        Proto.GetNotesResponse response = new();
        if (mediatorResponse.Notes is null)
        {
            return response;
        }
        response.Notes.AddRange(mediatorResponse.Notes.Select(ConvertNoteToPresentationNote));
        return response;
    }

    public override async Task<Proto.SetNotesResponse> SetNotes(Proto.SetNotesRequest request, ServerCallContext context)
    {
        Guid id = Guid.TryParse(request.Id, out Guid result) ? result : Guid.Empty;
        if (id == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID"));
        }

        Note[] notes = request.Notes.Select(ConvertPresentationNoteToNote).ToArray();

        ErrorOr<SetNotesResponse> mediatorResponse = await _sender.Send(new SetNotesRequest(id, notes));
        if (!mediatorResponse.IsError)
        {
            Proto.SetNotesResponse response = new();
            return response;
        }

        return mediatorResponse.FirstError.Type switch
        {
            ErrorType.NotFound => throw new RpcException(new Status(StatusCode.NotFound, mediatorResponse.FirstError.Description)),
            _ => throw new RpcException(new Status(StatusCode.Internal, mediatorResponse.FirstError.Description)),
        };
    }

    private static Proto.Note ConvertNoteToPresentationNote(Note note)
    {
        Proto.Note presentationNote = new()
        {
            Content = note.Content,
        };
        if (note.Children is not null)
        {
            presentationNote.Children.AddRange(note.Children.Select(ConvertNoteToPresentationNote));
        }
        return presentationNote;
    }

    private static Note ConvertPresentationNoteToNote(Proto.Note presentationNote)
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
