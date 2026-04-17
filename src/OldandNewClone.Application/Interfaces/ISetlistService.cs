using OldandNewClone.Application.DTOs;

namespace OldandNewClone.Application.Interfaces;

public interface ISetlistService
{
    Task<List<SetlistDto>> GetGlobalSetlistsAsync();
    Task<List<SetlistDto>> GetMySetlistsAsync(string userId);
    Task<List<SetlistDto>> GetSmartSetlistsAsync(string userId);
    Task<SetlistDto?> GetByIdAsync(string id);
    Task<SetlistDto> CreateAsync(string userId, bool isAdmin, CreateSetlistDto dto);
    Task<SetlistDto?> UpdateAsync(string userId, bool isAdmin, string id, UpdateSetlistDto dto);
    Task<bool> DeleteAsync(string userId, bool isAdmin, string id);
}
