using Application.Interfaces.Persistence;
using Domain.Entities;
using Domain.Models;
using MongoDB.Driver;

namespace Persistence.Common;

public sealed class NoteWriteRepository : INoteWriteRepository
{
    private readonly IMongoCollection<NoteRecord> _noteRecords;

    public NoteWriteRepository()
    {
        MongoClient client = new(Configurations.ConnectionStrings.MongoDb);
        IMongoDatabase database = client.GetDatabase("NoteTree");
        _noteRecords = database.GetCollection<NoteRecord>("Notes");
    }

    public async Task<Guid> CreateAsync(Note[] note)
    {
        NoteRecord newNoteRecord = new()
        {
            Notes = note
        };

        await _noteRecords.InsertOneAsync(newNoteRecord);

        return newNoteRecord.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        await _noteRecords.DeleteOneAsync(x => x.Id == id);
    }

    public async Task UpdateAsync(Guid id, Note[] note)
    {
        NoteRecord newNoteRecord = new()
        {
            Id = id,
            Notes = note
        };

        await _noteRecords.ReplaceOneAsync(x => x.Id == id, newNoteRecord);
    }
}
