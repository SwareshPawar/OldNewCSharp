using MongoDB.Driver;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;
using OldandNewClone.Infrastructure.Persistence;

namespace OldandNewClone.Infrastructure.Repositories;

public class SongRepository : ISongRepository
{
    private readonly MongoContext _context;

    public SongRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task<List<Song>> GetAllAsync()
    {
        return await _context.Songs.Find(_ => true).ToListAsync();
    }

    public async Task<Song?> GetByIdAsync(int songId)
    {
        return await _context.Songs.Find(s => s.SongId == songId).FirstOrDefaultAsync();
    }

    public async Task<Song> CreateAsync(Song song)
    {
        if (song.SongId == 0)
        {
            song.SongId = await GetNextIdAsync();
        }
        song.CreatedAt = DateTime.UtcNow;
        await _context.Songs.InsertOneAsync(song);
        return song;
    }

    public async Task<bool> UpdateAsync(Song song)
    {
        song.UpdatedAt = DateTime.UtcNow;
        var result = await _context.Songs.ReplaceOneAsync(s => s.SongId == song.SongId, song);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int songId)
    {
        var result = await _context.Songs.DeleteOneAsync(s => s.SongId == songId);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteAllAsync()
    {
        var result = await _context.Songs.DeleteManyAsync(_ => true);
        return result.DeletedCount > 0;
    }

    public async Task<List<Song>> SearchAsync(string searchTerm)
    {
        var filter = Builders<Song>.Filter.Or(
            Builders<Song>.Filter.Regex(s => s.Title, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Song>.Filter.Regex(s => s.Lyrics, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
        );
        return await _context.Songs.Find(filter).ToListAsync();
    }

    public async Task<List<Song>> GetByCategoryAsync(string category)
    {
        return await _context.Songs.Find(s => s.Category == category).ToListAsync();
    }

    public async Task<List<Song>> GetByKeyAsync(string key)
    {
        return await _context.Songs.Find(s => s.Key == key).ToListAsync();
    }

    public async Task<List<Song>> GetByGenresAsync(List<string> genres)
    {
        var filter = Builders<Song>.Filter.AnyIn(s => s.Genres, genres);
        return await _context.Songs.Find(filter).ToListAsync();
    }

    public async Task<int> GetNextIdAsync()
    {
        var lastSong = await _context.Songs
            .Find(_ => true)
            .SortByDescending(s => s.SongId)
            .Limit(1)
            .FirstOrDefaultAsync();

        return (lastSong?.SongId ?? 0) + 1;
    }
}
