using MongoDB.Bson;
using MongoDB.Driver;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;
using OldandNewClone.Infrastructure.Persistence;

namespace OldandNewClone.Infrastructure.Repositories;

public class SetlistRepository : ISetlistRepository
{
    private readonly MongoContext _context;

    public SetlistRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task<List<Setlist>> GetGlobalSetlistsAsync()
    {
        var items = await _context.GlobalSetlists.Find(FilterDefinition<Setlist>.Empty).SortBy(s => s.Name).ToListAsync();
        items.ForEach(s => s.Type = SetlistTypes.Global);
        return items;
    }

    public async Task<List<Setlist>> GetUserSetlistsAsync(string userId)
    {
        var items = await _context.MySetlists.Find(s => s.UserId == userId).SortBy(s => s.Name).ToListAsync();
        items.ForEach(s => s.Type = SetlistTypes.My);
        return items;
    }

    public async Task<List<Setlist>> GetSmartSetlistsAsync(string userId)
    {
        var items = await _context.SmartSetlists
            .Find(s => s.IsAdminCreated == true || s.CreatedBy == userId)
            .SortByDescending(s => s.CreatedAtRaw)
            .ToListAsync();
        items.ForEach(s => s.Type = SetlistTypes.Smart);
        return items;
    }

    public async Task<Setlist?> GetByIdAsync(string id)
    {
        var global = await _context.GlobalSetlists.Find(BuildIdFilter(id)).FirstOrDefaultAsync();
        if (global is not null) { global.Type = SetlistTypes.Global; return global; }

        var mine = await _context.MySetlists.Find(BuildIdFilter(id)).FirstOrDefaultAsync();
        if (mine is not null) { mine.Type = SetlistTypes.My; return mine; }

        var smart = await _context.SmartSetlists.Find(BuildIdFilter(id)).FirstOrDefaultAsync();
        if (smart is not null) { smart.Type = SetlistTypes.Smart; return smart; }

        return null;
    }

    public async Task<Setlist> CreateAsync(Setlist setlist)
    {
        var collection = GetCollection(setlist.Type);
        await collection.InsertOneAsync(setlist);
        return setlist;
    }

    public async Task<Setlist?> UpdateAsync(Setlist setlist)
    {
        var collection = GetCollection(setlist.Type);
        var result = await collection.ReplaceOneAsync(BuildIdFilter(setlist.Id), setlist);
        return result.MatchedCount == 0 ? null : setlist;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var existing = await GetByIdAsync(id);
        if (existing is null) return false;

        var collection = GetCollection(existing.Type);
        var result = await collection.DeleteOneAsync(BuildIdFilter(id));
        return result.DeletedCount > 0;
    }

    private IMongoCollection<Setlist> GetCollection(string type) => type switch
    {
        SetlistTypes.Global => _context.GlobalSetlists,
        SetlistTypes.Smart => _context.SmartSetlists,
        _ => _context.MySetlists
    };

    private static FilterDefinition<Setlist> BuildIdFilter(string id)
    {
        if (ObjectId.TryParse(id, out var objectId))
        {
            return Builders<Setlist>.Filter.Or(
                Builders<Setlist>.Filter.Eq("_id", objectId),
                Builders<Setlist>.Filter.Eq("_id", id));
        }

        return Builders<Setlist>.Filter.Eq("_id", id);
    }
}
