using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Interfaces;

public interface ISongRepository
{
    Task<List<Song>> GetAllAsync();
    Task<Song?> GetByIdAsync(int songId);
    Task<Song> CreateAsync(Song song);
    Task<bool> UpdateAsync(Song song);
    Task<bool> DeleteAsync(int songId);
    Task<bool> DeleteAllAsync();
    Task<List<Song>> SearchAsync(string searchTerm);
    Task<List<Song>> GetByCategoryAsync(string category);
    Task<List<Song>> GetByKeyAsync(string key);
    Task<List<Song>> GetByGenresAsync(List<string> genres);
    Task<int> GetNextIdAsync();
}
