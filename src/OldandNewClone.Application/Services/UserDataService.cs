using OldandNewClone.Application.DTOs;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Services;

public class UserDataService : IUserDataService
{
    private readonly IUserDataRepository _userDataRepository;

    public UserDataService(IUserDataRepository userDataRepository)
    {
        _userDataRepository = userDataRepository;
    }

    public async Task<UserDataDto?> GetUserDataAsync(string userId)
    {
        var userData = await _userDataRepository.GetByUserIdAsync(userId);

        if (userData == null)
        {
            return new UserDataDto(userId, null, null, new List<int>(), new List<int>(), new List<int>());
        }

        return new UserDataDto(
            userData.UserId,
            userData.Name,
            userData.Email,
            userData.Favorites,
            userData.NewSetlist,
            userData.OldSetlist
        );
    }

    public async Task<UserDataDto> UpdateUserDataAsync(string userId, UpdateUserDataDto updateDto)
    {
        var userData = new UserData
        {
            UserId = userId,
            Name = updateDto.Name,
            Email = updateDto.Email,
            Favorites = updateDto.Favorites,
            NewSetlist = updateDto.NewSetlist,
            OldSetlist = updateDto.OldSetlist
        };

        var updated = await _userDataRepository.UpsertAsync(userData);

        return new UserDataDto(
            updated.UserId,
            updated.Name,
            updated.Email,
            updated.Favorites,
            updated.NewSetlist,
            updated.OldSetlist
        );
    }

    public async Task<bool> ToggleFavoriteAsync(string userId, int songId)
    {
        var userData = await _userDataRepository.GetByUserIdAsync(userId);

        if (userData == null)
        {
            userData = new UserData { UserId = userId, Favorites = new List<int> { songId } };
            await _userDataRepository.UpsertAsync(userData);
            return true;
        }

        if (userData.Favorites.Contains(songId))
        {
            return await _userDataRepository.RemoveFavoriteAsync(userId, songId);
        }
        else
        {
            return await _userDataRepository.AddFavoriteAsync(userId, songId);
        }
    }

    public async Task<bool> ToggleNewSetlistAsync(string userId, int songId)
    {
        var userData = await _userDataRepository.GetByUserIdAsync(userId);

        if (userData == null)
        {
            userData = new UserData { UserId = userId, NewSetlist = new List<int> { songId } };
            await _userDataRepository.UpsertAsync(userData);
            return true;
        }

        if (userData.NewSetlist.Contains(songId))
        {
            return await _userDataRepository.RemoveFromNewSetlistAsync(userId, songId);
        }
        else
        {
            return await _userDataRepository.AddToNewSetlistAsync(userId, songId);
        }
    }

    public async Task<bool> ToggleOldSetlistAsync(string userId, int songId)
    {
        var userData = await _userDataRepository.GetByUserIdAsync(userId);

        if (userData == null)
        {
            userData = new UserData { UserId = userId, OldSetlist = new List<int> { songId } };
            await _userDataRepository.UpsertAsync(userData);
            return true;
        }

        if (userData.OldSetlist.Contains(songId))
        {
            return await _userDataRepository.RemoveFromOldSetlistAsync(userId, songId);
        }
        else
        {
            return await _userDataRepository.AddToOldSetlistAsync(userId, songId);
        }
    }
}
