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
        return await _context.UserData.Find(u => u.UserId == userId).FirstOrDefaultAsync();
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
}
