using OldandNewClone.Application.DTOs;

namespace OldandNewClone.Application.Interfaces;

public interface ISongService
{
    Task<List<SongListItemDto>> GetAllSongsAsync();
    Task<SongDto?> GetSongByIdAsync(int songId);
    Task<SongDto> CreateSongAsync(CreateSongDto createSongDto);
    Task<SongDto?> UpdateSongAsync(UpdateSongDto updateSongDto);
    Task<bool> DeleteSongAsync(int songId);
    Task<List<SongListItemDto>> SearchSongsAsync(string searchTerm);
    Task<List<SongListItemDto>> FilterSongsAsync(string? category, string? key, List<string>? genres);
}
