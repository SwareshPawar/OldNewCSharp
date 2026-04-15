using OldandNewClone.Application.DTOs;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Services;

public class SongService : ISongService
{
    private readonly ISongRepository _songRepository;

    public SongService(ISongRepository songRepository)
    {
        _songRepository = songRepository;
    }

    public async Task<List<SongListItemDto>> GetAllSongsAsync()
    {
        var songs = await _songRepository.GetAllAsync();
        return songs.Select(s => new SongListItemDto(
            s.SongId,
            s.Title,
            s.Category,
            s.Key,
            s.Genres
        )).ToList();
    }

    public async Task<SongDto?> GetSongByIdAsync(int songId)
    {
        var song = await _songRepository.GetByIdAsync(songId);
        if (song == null) return null;

        return new SongDto(
            song.SongId,
            song.Title,
            song.Category,
            song.Key,
            song.Tempo,
            song.Time,
            song.Taal,
            song.Genres,
            song.Lyrics
        );
    }

    public async Task<SongDto> CreateSongAsync(CreateSongDto createSongDto)
    {
        var song = new Song
        {
            Title = createSongDto.Title,
            Category = createSongDto.Category,
            Key = createSongDto.Key,
            Tempo = createSongDto.Tempo,
            Time = createSongDto.Time,
            Taal = createSongDto.Taal,
            Genres = createSongDto.Genres,
            Lyrics = createSongDto.Lyrics
        };

        var created = await _songRepository.CreateAsync(song);

        return new SongDto(
            created.SongId,
            created.Title,
            created.Category,
            created.Key,
            created.Tempo,
            created.Time,
            created.Taal,
            created.Genres,
            created.Lyrics
        );
    }

    public async Task<bool> UpdateSongAsync(UpdateSongDto updateSongDto)
    {
        var existing = await _songRepository.GetByIdAsync(updateSongDto.Id);
        if (existing == null) return false;

        existing.Title = updateSongDto.Title;
        existing.Category = updateSongDto.Category;
        existing.Key = updateSongDto.Key;
        existing.Tempo = updateSongDto.Tempo;
        existing.Time = updateSongDto.Time;
        existing.Taal = updateSongDto.Taal;
        existing.Genres = updateSongDto.Genres;
        existing.Lyrics = updateSongDto.Lyrics;

        return await _songRepository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteSongAsync(int songId)
    {
        return await _songRepository.DeleteAsync(songId);
    }

    public async Task<List<SongListItemDto>> SearchSongsAsync(string searchTerm)
    {
        var songs = await _songRepository.SearchAsync(searchTerm);
        return songs.Select(s => new SongListItemDto(
            s.SongId,
            s.Title,
            s.Category,
            s.Key,
            s.Genres
        )).ToList();
    }

    public async Task<List<SongListItemDto>> FilterSongsAsync(string? category, string? key, List<string>? genres)
    {
        var allSongs = await _songRepository.GetAllAsync();

        var filtered = allSongs.AsEnumerable();

        if (!string.IsNullOrEmpty(category))
        {
            filtered = filtered.Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(key))
        {
            filtered = filtered.Where(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        if (genres != null && genres.Any())
        {
            filtered = filtered.Where(s => s.Genres.Intersect(genres, StringComparer.OrdinalIgnoreCase).Any());
        }

        return filtered.Select(s => new SongListItemDto(
            s.SongId,
            s.Title,
            s.Category,
            s.Key,
            s.Genres
        )).ToList();
    }
}
