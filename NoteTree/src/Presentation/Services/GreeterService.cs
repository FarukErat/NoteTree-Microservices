namespace Presentation.Services;

using Grpc.Core;
using MediatR;

using Application.Mediator.GetNotes;
using Application.Mediator.SetNotes;
using Application.Mediator.CreateEmptyNoteRecord;

using Domain.Models;
using ErrorOr;

public sealed class NoteTreeService(
    ISender sender)
    : NoteTree.NoteTreeBase
{
    private readonly ISender _sender = sender;

    public override async Task<Presentation.CreateEmptyNoteRecordResponse> CreateEmptyNoteRecord(Presentation.CreateEmptyNoteRecordRequest request, ServerCallContext context)
    {
        CreateEmptyNoteRecordResponse mediatorResponse = await _sender.Send(
            new CreateEmptyNoteRecordRequest());

        Presentation.CreateEmptyNoteRecordResponse response = new()
        {
            Id = mediatorResponse.Id.ToString(),
        };

        return response;
    }

    public override async Task<Presentation.GetNotesResponse> GetNotes(Presentation.GetNotesRequest request, ServerCallContext context)
    {
        Guid id = Guid.TryParse(request.Id, out Guid result) ? result : Guid.Empty;
        if (id == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID"));
        }

        GetNotesResponse mediatorResponse = await _sender.Send(
            new GetNotesRequest(id));

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
        Guid id = Guid.TryParse(request.Id, out Guid result) ? result : Guid.Empty;
        if (id == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid ID"));
        }

        Note[] notes = request.Notes.Select(ConvertPresentationNoteToNote).ToArray();

        ErrorOr<SetNotesResponse> mediatorResponse = await _sender.Send(new SetNotesRequest(id, notes));
        if (!mediatorResponse.IsError)
        {
            Presentation.SetNotesResponse response = new();
            return response;
        }

        return mediatorResponse.FirstError.Type switch
        {
            ErrorType.NotFound => throw new RpcException(new Status(StatusCode.NotFound, mediatorResponse.FirstError.Description)),
            _ => throw new RpcException(new Status(StatusCode.Internal, mediatorResponse.FirstError.Description)),
        };
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
