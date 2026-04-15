using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Interfaces;

public interface IUserDataRepository
{
    Task<UserData?> GetByUserIdAsync(string userId);
    Task<UserData> UpsertAsync(UserData userData);
    Task<bool> AddFavoriteAsync(string userId, int songId);
    Task<bool> RemoveFavoriteAsync(string userId, int songId);
    Task<bool> AddToNewSetlistAsync(string userId, int songId);
    Task<bool> RemoveFromNewSetlistAsync(string userId, int songId);
    Task<bool> AddToOldSetlistAsync(string userId, int songId);
    Task<bool> RemoveFromOldSetlistAsync(string userId, int songId);
}
