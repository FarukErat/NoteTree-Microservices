using ErrorOr;
using Features.NoteTree.Domain.Models;
using Features.NoteTree.GetNotes.Proto;
using Grpc.Core;
using Grpc.Net.Client;
using Infrastructure;

using static Features.NoteTree.GetNotes.Proto.NoteTree;
using Note = Features.NoteTree.Domain.Models.Note; // conflicts with Features.NoteTree.GetNotes.Proto.Note

namespace Features.NoteTree.GetNotes;

public sealed class GetNotesService
{
    private readonly GrpcChannel _channel;
    private readonly NoteTreeClient _client;

    public GetNotesService()
    {
        _channel = GrpcChannel.ForAddress(Configurations.ConnectionStrings.NoteTreeServiceUrl);
        _client = new NoteTreeClient(_channel);
    }

    public async Task<ErrorOr<Note[]>> GetNotes(string jwt)
    {
        Metadata headers = [new Metadata.Entry("Authorization", "Bearer " + jwt)];
        Proto.GetNotesRequest request = new();
        try
        {
            Proto.GetNotesResponse response = await _client.GetNotesAsync(request, headers);
            return response.Notes.Select(ConvertProtoNoteToNote).ToArray();
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
