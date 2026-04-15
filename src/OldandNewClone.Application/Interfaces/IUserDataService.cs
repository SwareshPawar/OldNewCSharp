using OldandNewClone.Application.DTOs;

namespace OldandNewClone.Application.Interfaces;

public interface IUserDataService
{
    Task<UserDataDto?> GetUserDataAsync(string userId);
    Task<UserDataDto> UpdateUserDataAsync(string userId, UpdateUserDataDto updateDto);
    Task<bool> ToggleFavoriteAsync(string userId, int songId);
    Task<bool> ToggleNewSetlistAsync(string userId, int songId);
    Task<bool> ToggleOldSetlistAsync(string userId, int songId);
}
