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
        return songs.Select(MapToListItem).ToList();
    }

    public async Task<SongDto?> GetSongByIdAsync(int songId)
    {
        var song = await _songRepository.GetByIdAsync(songId);
        return song == null ? null : MapToDto(song);
    }

    public async Task<SongDto> CreateSongAsync(CreateSongDto dto)
    {
        var song = new Song
        {
            Title = dto.Title,
            Category = dto.Category,
            Key = dto.Key,
            Tempo = dto.Tempo,
            Time = dto.Time,
            Taal = dto.Taal,
            Genres = dto.Genres,
            Lyrics = dto.Lyrics,
            Singer = dto.Singer,
            Mood = dto.Mood,
            Tags = dto.Tags,
            YoutubeLink = dto.YoutubeLink,
            SpotifyLink = dto.SpotifyLink,
            Notes = dto.Notes,
            IsPublic = dto.IsPublic
        };

        var created = await _songRepository.CreateAsync(song);
        return MapToDto(created);
    }

    public async Task<bool> UpdateSongAsync(UpdateSongDto dto)
    {
        var existing = await _songRepository.GetByIdAsync(dto.Id);
        if (existing == null) return false;

        existing.Title = dto.Title;
        existing.Category = dto.Category;
        existing.Key = dto.Key;
        existing.Tempo = dto.Tempo;
        existing.Time = dto.Time;
        existing.Taal = dto.Taal;
        existing.Genres = dto.Genres;
        existing.Lyrics = dto.Lyrics;
        existing.Singer = dto.Singer;
        existing.Mood = dto.Mood;
        existing.Tags = dto.Tags;
        existing.YoutubeLink = dto.YoutubeLink;
        existing.SpotifyLink = dto.SpotifyLink;
        existing.Notes = dto.Notes;
        existing.IsPublic = dto.IsPublic;

        return await _songRepository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteSongAsync(int songId)
    {
        return await _songRepository.DeleteAsync(songId);
    }

    public async Task<List<SongListItemDto>> SearchSongsAsync(string searchTerm)
    {
        var songs = await _songRepository.SearchAsync(searchTerm);
        return songs.Select(MapToListItem).ToList();
    }

    public async Task<List<SongListItemDto>> FilterSongsAsync(string? category, string? key, List<string>? genres)
    {
        var allSongs = await _songRepository.GetAllAsync();

        var filtered = allSongs.AsEnumerable();

        if (!string.IsNullOrEmpty(category))
            filtered = filtered.Where(s => s.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrEmpty(key))
            filtered = filtered.Where(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (genres != null && genres.Any())
            filtered = filtered.Where(s => s.Genres.Intersect(genres, StringComparer.OrdinalIgnoreCase).Any());

        return filtered.Select(MapToListItem).ToList();
    }

    private static SongListItemDto MapToListItem(Song s) => new(
        s.SongId, s.Title, s.Category, s.Key, s.Genres, s.Singer, s.Mood, s.Tempo, s.Time, s.Taal, s.Tags);

    private static SongDto MapToDto(Song s) => new(
        s.SongId, s.Title, s.Category, s.Key,
        s.Tempo, s.Time, s.Taal, s.Genres, s.Lyrics,
        s.Singer, s.Mood, s.Tags, s.YoutubeLink,
        s.SpotifyLink, s.Notes, s.IsPublic, s.ViewCount,
        s.CreatedAt, s.UpdatedAt);
}
