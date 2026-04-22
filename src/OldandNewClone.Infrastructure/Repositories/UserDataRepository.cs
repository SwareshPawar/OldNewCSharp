using MongoDB.Bson;
using MongoDB.Driver;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;
using OldandNewClone.Infrastructure.Persistence;

namespace OldandNewClone.Infrastructure.Repositories;

public class UserDataRepository : IUserDataRepository
{
    private readonly MongoContext _context;

    public UserDataRepository(MongoContext context)
    {
        _context = context;
    }

    public async Task<UserData?> GetByUserIdAsync(string userId)
    {
        try
        {
            return await _context.UserData.Find(u => u.UserId == userId).FirstOrDefaultAsync();
        }
        catch (FormatException)
        {
            var collection = _context.Database.GetCollection<BsonDocument>(_context.UserData.CollectionNamespace.CollectionName);
            var doc = await collection.Find(Builders<BsonDocument>.Filter.Eq("_id", userId)).FirstOrDefaultAsync();
            if (doc == null) return null;

            return new UserData
            {
                UserId = doc.GetValue("_id", BsonNull.Value).ToString() ?? string.Empty,
                Name = doc.GetValue("name", BsonNull.Value).IsBsonNull ? null : doc.GetValue("name", BsonNull.Value).AsString,
                Email = doc.GetValue("email", BsonNull.Value).IsBsonNull ? null : doc.GetValue("email", BsonNull.Value).AsString,
                Favorites = ParseSongIds(doc.GetValue("favorites", new BsonArray())),
                NewSetlist = ParseSongIds(doc.GetValue("NewSetlist", new BsonArray())),
                OldSetlist = ParseSongIds(doc.GetValue("OldSetlist", new BsonArray())),
                UpdatedAt = DateTime.UtcNow
            };
        }
    }

    public async Task<UserData> UpsertAsync(UserData userData)
    {
        userData.UpdatedAt = DateTime.UtcNow;

        var options = new ReplaceOptions { IsUpsert = true };
        await _context.UserData.ReplaceOneAsync(u => u.UserId == userData.UserId, userData, options);

        return userData;
    }

    public async Task<bool> AddFavoriteAsync(string userId, int songId)
    {
        var update = Builders<UserData>.Update.AddToSet(u => u.Favorites, songId);
        var result = await _context.UserData.UpdateOneAsync(u => u.UserId == userId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveFavoriteAsync(string userId, int songId)
    {
        var update = Builders<UserData>.Update.Pull(u => u.Favorites, songId);
        var result = await _context.UserData.UpdateOneAsync(u => u.UserId == userId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> AddToNewSetlistAsync(string userId, int songId)
    {
        var update = Builders<UserData>.Update.AddToSet(u => u.NewSetlist, songId);
        var result = await _context.UserData.UpdateOneAsync(u => u.UserId == userId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveFromNewSetlistAsync(string userId, int songId)
    {
        var update = Builders<UserData>.Update.Pull(u => u.NewSetlist, songId);
        var result = await _context.UserData.UpdateOneAsync(u => u.UserId == userId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> AddToOldSetlistAsync(string userId, int songId)
    {
        var update = Builders<UserData>.Update.AddToSet(u => u.OldSetlist, songId);
        var result = await _context.UserData.UpdateOneAsync(u => u.UserId == userId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> RemoveFromOldSetlistAsync(string userId, int songId)
    {
        var update = Builders<UserData>.Update.Pull(u => u.OldSetlist, songId);
        var result = await _context.UserData.UpdateOneAsync(u => u.UserId == userId, update);
        return result.ModifiedCount > 0;
    }

    private static List<int> ParseSongIds(BsonValue raw)
    {
        if (!raw.IsBsonArray) return new List<int>();

        return raw.AsBsonArray
            .Select(ParseSongId)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
    }

    private static int? ParseSongId(BsonValue value)
    {
        if (value.IsInt32) return value.AsInt32;
        if (value.IsInt64) return (int)value.AsInt64;
        if (value.IsString && int.TryParse(value.AsString, out var fromString)) return fromString;

        if (value.IsBsonDocument)
        {
            var doc = value.AsBsonDocument;
            if (doc.TryGetValue("id", out var idValue) && TryParseInt(idValue, out var parsedId)) return parsedId;
            if (doc.TryGetValue("Id", out var upperIdValue) && TryParseInt(upperIdValue, out parsedId)) return parsedId;
            if (doc.TryGetValue("songId", out var songIdValue) && TryParseInt(songIdValue, out parsedId)) return parsedId;
        }

        return null;
    }

    private static bool TryParseInt(BsonValue value, out int parsed)
    {
        parsed = default;
        if (value.IsInt32)
        {
            parsed = value.AsInt32;
            return true;
        }

        if (value.IsInt64)
        {
            parsed = (int)value.AsInt64;
            return true;
        }

        return value.IsString && int.TryParse(value.AsString, out parsed);
    }
}
