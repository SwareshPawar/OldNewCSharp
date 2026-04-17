using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Interfaces;

public interface ISetlistRepository
{
    Task<List<Setlist>> GetGlobalSetlistsAsync();
    Task<List<Setlist>> GetUserSetlistsAsync(string userId);
    Task<List<Setlist>> GetSmartSetlistsAsync(string userId);
    Task<Setlist?> GetByIdAsync(string id);
    Task<Setlist> CreateAsync(Setlist setlist);
    Task<Setlist?> UpdateAsync(Setlist setlist);
    Task<bool> DeleteAsync(string id);
}
