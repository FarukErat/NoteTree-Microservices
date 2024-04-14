using Application.Interfaces.Persistence;
using Domain.Entities;
using Domain.Models;
using MongoDB.Driver;

namespace Persistence.Common;

public sealed class NoteReadRepository : INoteReadRepository
{
    private readonly IMongoCollection<NoteRecord> _noteRecords;

    public NoteReadRepository()
    {
        MongoClient client = new(Configurations.ConnectionStrings.MongoDbConnectionString);
        IMongoDatabase database = client.GetDatabase("NoteTree");
        _noteRecords = database.GetCollection<NoteRecord>("Notes");
    }

    public async Task<Note[]?> GetByIdAsync(Guid id)
    {
        NoteRecord? noteRecord = await _noteRecords.Find(x => x.Id == id).FirstOrDefaultAsync();
        return noteRecord?.Notes;
    }
}
