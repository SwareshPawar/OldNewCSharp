using MongoDB.Bson;
using OldandNewClone.Application.DTOs;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Services;

public class SetlistService : ISetlistService
{
    private readonly ISetlistRepository _setlistRepository;

    public SetlistService(ISetlistRepository setlistRepository)
    {
        _setlistRepository = setlistRepository;
    }

    public async Task<List<SetlistDto>> GetGlobalSetlistsAsync() =>
        (await _setlistRepository.GetGlobalSetlistsAsync()).Select(Map).ToList();

    public async Task<List<SetlistDto>> GetMySetlistsAsync(string userId) =>
        (await _setlistRepository.GetUserSetlistsAsync(userId)).Select(Map).ToList();

    public async Task<List<SetlistDto>> GetSmartSetlistsAsync(string userId) =>
        (await _setlistRepository.GetSmartSetlistsAsync(userId)).Select(Map).ToList();

    public async Task<SetlistDto?> GetByIdAsync(string id)
    {
        var setlist = await _setlistRepository.GetByIdAsync(id);
        return setlist is null ? null : Map(setlist);
    }

    public async Task<SetlistDto> CreateAsync(string userId, bool isAdmin, CreateSetlistDto dto)
    {
        if (dto.Type == SetlistTypes.Global && !isAdmin)
            throw new UnauthorizedAccessException("Only administrators can create global setlists.");

        var setlist = new Setlist
        {
            Type = dto.Type,
            Name = dto.Name,
            Description = dto.Description,
            UserId = dto.Type == SetlistTypes.My ? userId : null,
            CreatedBy = dto.Type == SetlistTypes.Smart ? userId : userId,
            IsAdminCreated = dto.Type == SetlistTypes.Smart ? isAdmin : null,
            SongsRaw = (dto.Songs ?? new List<int>()).Select(id => (BsonValue)id).ToList(),
            ConditionsRaw = dto.Conditions is null ? null : new MongoDB.Bson.BsonDocument(dto.Conditions),
            CreatedAtRaw = DateTime.UtcNow,
            UpdatedAtRaw = DateTime.UtcNow
        };

        var created = await _setlistRepository.CreateAsync(setlist);
        return Map(created);
    }

    public async Task<SetlistDto?> UpdateAsync(string userId, bool isAdmin, string id, UpdateSetlistDto dto)
    {
        var existing = await _setlistRepository.GetByIdAsync(id);
        if (existing is null)
            return null;

        if (existing.Type == SetlistTypes.Global)
        {
            if (!isAdmin)
                throw new UnauthorizedAccessException("Only administrators can modify global setlists.");
        }
        else if (existing.Type == SetlistTypes.My && existing.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only modify your own setlists.");
        }
        else if (existing.Type == SetlistTypes.Smart && existing.CreatedBy != userId && !(isAdmin && existing.IsAdminCreated == true))
        {
            throw new UnauthorizedAccessException("You do not have permission to modify this smart setlist.");
        }

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.SongsRaw = (dto.Songs ?? new List<int>()).Select(id => (BsonValue)id).ToList();
        existing.ConditionsRaw = dto.Conditions is null ? null : new MongoDB.Bson.BsonDocument(dto.Conditions);
        existing.UpdatedAtRaw = DateTime.UtcNow;

        var updated = await _setlistRepository.UpdateAsync(existing);
        return updated is null ? null : Map(updated);
    }

    public async Task<bool> DeleteAsync(string userId, bool isAdmin, string id)
    {
        var existing = await _setlistRepository.GetByIdAsync(id);
        if (existing is null)
            return false;

        if (existing.Type == SetlistTypes.Global)
        {
            if (!isAdmin)
                throw new UnauthorizedAccessException("Only administrators can delete global setlists.");
        }
        else if (existing.Type == SetlistTypes.My && existing.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only delete your own setlists.");
        }
        else if (existing.Type == SetlistTypes.Smart && existing.CreatedBy != userId && !(isAdmin && existing.IsAdminCreated == true))
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this smart setlist.");
        }

        return await _setlistRepository.DeleteAsync(id);
    }

    private static SetlistDto Map(Setlist setlist) => new(
        setlist.Id,
        setlist.Name,
        setlist.Description,
        setlist.Type,
        setlist.UserId ?? setlist.CreatedBy,
        setlist.SongIds,
        setlist.Conditions,
        setlist.Type == SetlistTypes.Global || setlist.IsAdminCreated == true,
        setlist.CreatedAt,
        setlist.UpdatedAt
    );
}
