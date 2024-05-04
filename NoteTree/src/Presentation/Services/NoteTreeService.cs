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
        string? bearerToken = context.RequestHeaders.GetValue("Authorization");
        string? jwt = bearerToken?.Split(" ")[1];
        if(string.IsNullOrEmpty(jwt))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "No token provided"));
        }

        GetNotesRequest mediatorRequest = new(jwt);
        ErrorOr<GetNotesResponse> mediatorResponse = await _sender.Send(mediatorRequest);
        if (!mediatorResponse.IsError)
        {
            Proto.GetNotesResponse response = new();
            response.Notes.AddRange(mediatorResponse.Value.Notes.Select(ConvertNoteToProtoNote));
            return response;
        }
        switch (mediatorResponse.FirstError.Type)
        {
            case ErrorType.Validation:
                throw new RpcException(new Status(StatusCode.InvalidArgument, mediatorResponse.FirstError.Description));
            case ErrorType.NotFound:
                throw new RpcException(new Status(StatusCode.NotFound, mediatorResponse.FirstError.Description));
            case ErrorType.Unauthorized:
                throw new RpcException(new Status(StatusCode.PermissionDenied, mediatorResponse.FirstError.Description));
            default:
                throw new RpcException(new Status(StatusCode.Internal, mediatorResponse.FirstError.Description));
        }
    }

    public override async Task<Proto.SetNotesResponse> SetNotes(Proto.SetNotesRequest request, ServerCallContext context)
    {
        string? bearerToken = context.RequestHeaders.GetValue("Authorization");
        string? jwt = bearerToken?.Split(" ")[1];
        if(string.IsNullOrEmpty(jwt))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "No token provided"));
        }

        Note[] notes = request.Notes.Select(ConvertProtoNoteToNote).ToArray();
        SetNotesRequest mediatorRequest = new(jwt, notes);
        ErrorOr<SetNotesResponse> mediatorResponse = await _sender.Send(mediatorRequest);
        if (!mediatorResponse.IsError)
        {
            Proto.SetNotesResponse response = new();
            return response;
        }
        switch (mediatorResponse.FirstError.Type)
        {
            case ErrorType.Validation:
                throw new RpcException(new Status(StatusCode.InvalidArgument, mediatorResponse.FirstError.Description));
            case ErrorType.NotFound:
                throw new RpcException(new Status(StatusCode.NotFound, mediatorResponse.FirstError.Description));
            case ErrorType.Unauthorized:
                throw new RpcException(new Status(StatusCode.PermissionDenied, mediatorResponse.FirstError.Description));
            default:
                throw new RpcException(new Status(StatusCode.Internal, mediatorResponse.FirstError.Description));
        }
    }

    private static Proto.Note ConvertNoteToProtoNote(Note note)
    {
        Proto.Note presentationNote = new()
        {
            Content = note.Content,
        };
        if (note.Children is not null)
        {
            presentationNote.Children.AddRange(note.Children.Select(ConvertNoteToProtoNote));
        }
        return presentationNote;
    }

    private static Note ConvertProtoNoteToNote(Proto.Note presentationNote)
    {
        Note note = new()
        {
            Content = presentationNote.Content,
        };
        if (presentationNote.Children is not null)
        {
            note.Children = presentationNote.Children.Select(ConvertProtoNoteToNote).ToArray();
        }
        return note;
    }
}
