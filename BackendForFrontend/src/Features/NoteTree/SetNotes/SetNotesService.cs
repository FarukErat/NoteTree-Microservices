using ErrorOr;
using Features.NoteTree.Domain.Models;
using Features.NoteTree.SetNotes.Proto;
using Grpc.Core;
using Grpc.Net.Client;
using Infrastructure;

using static Features.NoteTree.SetNotes.Proto.NoteTree;
using Note = Features.NoteTree.Domain.Models.Note; // conflicts with Features.NoteTree.GetNotes.Proto.Note

namespace Features.NoteTree.SetNotes;

public sealed class SetNotesService
{
    private readonly GrpcChannel _channel;
    private readonly NoteTreeClient _client;

    public SetNotesService()
    {
        _channel = GrpcChannel.ForAddress(Configurations.AuthenticationUrl);
        _client = new NoteTreeClient(_channel);
    }

    public async Task<ErrorOr<Success>> SetNotes(string jwt, Note[] notes)
    {
        Proto.SetNotesRequest request = new()
        {
            Jwt = jwt,
            Notes = { notes.Select(ConvertNoteToProtoNote) },
        };
        try
        {
            await _client.SetNotesAsync(request);
            return Result.Success;
        }
        catch (RpcException e)
        {
            switch (e.StatusCode)
            {
                case StatusCode.InvalidArgument:
                    return Error.Validation(e.Message);
                case StatusCode.NotFound:
                    return Error.NotFound(e.Message);
                case StatusCode.Unauthenticated:
                    return Error.Unauthorized(e.Message);
                default:
                    return Error.Unexpected(e.Message);
            }
        }
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
}
