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

        if (mediatorResponse.IsError)
        {
            throw CreateRpcException(mediatorResponse.FirstError);
        }

        Proto.GetNotesResponse response = new();
        response.Notes.AddRange(mediatorResponse.Value.Notes.Select(ConvertNoteToProtoNote));

        return response;
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

        if (mediatorResponse.IsError)
        {
            throw CreateRpcException(mediatorResponse.FirstError);
        }

        Proto.SetNotesResponse response = new();
        return response;
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

    private static RpcException CreateRpcException(Error error)
    {
        switch (error.Type)
        {
            case ErrorType.Conflict:
                return new RpcException(new Status(StatusCode.AlreadyExists, error.Description));
            case ErrorType.NotFound:
                return new RpcException(new Status(StatusCode.NotFound, error.Description));
            case ErrorType.Validation:
                return new RpcException(new Status(StatusCode.InvalidArgument, error.Description));
            case ErrorType.Unauthorized:
                return new RpcException(new Status(StatusCode.Unauthenticated, error.Description));
            case ErrorType.Forbidden:
                return new RpcException(new Status(StatusCode.PermissionDenied, error.Description));
            case ErrorType.Failure:
            case ErrorType.Unexpected:
            default:
                return new RpcException(new Status(StatusCode.Internal, error.Description));
        }
    }
}
