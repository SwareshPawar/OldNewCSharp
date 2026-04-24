using MongoDB.Bson;
using OldandNewClone.Application.DTOs;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Services;

public class SetlistService : ISetlistService
{
    private readonly ISetlistRepository _setlistRepository;
    private readonly ISongRepository _songRepository;

    public SetlistService(ISetlistRepository setlistRepository, ISongRepository songRepository)
    {
        _setlistRepository = setlistRepository;
        _songRepository = songRepository;
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
        else if (existing.Type == SetlistTypes.Smart && existing.CreatedBy != userId && !isAdmin)
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

    public async Task<SetlistDto?> AddSongAsync(string userId, bool isAdmin, string id, int songId)
    {
        var existing = await _setlistRepository.GetByIdAsync(id);
        if (existing is null)
            return null;

        EnsureCanManuallyEditSongs(existing, userId, isAdmin);

        if (!existing.SongIds.Contains(songId))
        {
            existing.SongsRaw.Add(songId);
            existing.UpdatedAtRaw = DateTime.UtcNow;
        }

        var updated = await _setlistRepository.UpdateAsync(existing);
        return updated is null ? null : Map(updated);
    }

    public async Task<SetlistDto?> RemoveSongAsync(string userId, bool isAdmin, string id, int songId)
    {
        var existing = await _setlistRepository.GetByIdAsync(id);
        if (existing is null)
            return null;

        EnsureCanManuallyEditSongs(existing, userId, isAdmin);

        existing.SongsRaw = existing.SongsRaw.Where(value => !MatchesSongId(value, songId)).ToList();
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
        else if (existing.Type == SetlistTypes.Smart && existing.CreatedBy != userId && !isAdmin)
        {
            throw new UnauthorizedAccessException("You do not have permission to delete this smart setlist.");
        }

        return await _setlistRepository.DeleteAsync(id);
    }

    public async Task<SetlistDto?> SyncSmartSetlistAsync(string userId, bool isAdmin, string id)
    {
        var existing = await _setlistRepository.GetByIdAsync(id);
        if (existing is null) return null;
        if (existing.Type != SetlistTypes.Smart)
            throw new InvalidOperationException("Sync is supported only for smart setlists.");

        if (existing.CreatedBy != userId && !isAdmin)
            throw new UnauthorizedAccessException("You do not have permission to sync this smart setlist.");

        var songs = await _songRepository.GetAllAsync();
        var filtered = ApplySmartConditions(songs, existing.Conditions);
        existing.SongsRaw = filtered.Select(s => (BsonValue)s.SongId).ToList();
        existing.UpdatedAtRaw = DateTime.UtcNow;

        var updated = await _setlistRepository.UpdateAsync(existing);
        return updated is null ? null : Map(updated);
    }

    private static List<Song> ApplySmartConditions(List<Song> songs, Dictionary<string, string>? conditions)
    {
        if (conditions is null || conditions.Count == 0) return songs;

        IEnumerable<Song> query = songs;

        var categories = ReadCsv(conditions, "categories");
        if (categories.Count > 0)
            query = query.Where(s => categories.Contains(s.Category, StringComparer.OrdinalIgnoreCase));

        var keys = ReadCsv(conditions, "keys");
        if (keys.Count > 0)
            query = query.Where(s => keys.Contains(s.Key, StringComparer.OrdinalIgnoreCase));

        var moods = ReadCsv(conditions, "moods");
        if (moods.Count > 0)
            query = query.Where(s => moods.Any(m => (s.Mood ?? string.Empty).Contains(m, StringComparison.OrdinalIgnoreCase)));

        var genres = ReadCsv(conditions, "genres");
        if (genres.Count > 0)
            query = query.Where(s => s.Genres.Any(g => genres.Contains(g, StringComparer.OrdinalIgnoreCase)));

        var times = ReadCsv(conditions, "times");
        if (times.Count > 0)
            query = query.Where(s => times.Contains(s.Time, StringComparer.OrdinalIgnoreCase));

        var taals = ReadCsv(conditions, "taals");
        if (taals.Count > 0)
            query = query.Where(s => taals.Contains(s.Taal, StringComparer.OrdinalIgnoreCase));

        var tempoMin = 0;
        var tempoMax = 0;
        var hasTempoMin = conditions.TryGetValue("tempoMin", out var tempoMinRaw) && int.TryParse(tempoMinRaw, out tempoMin);
        var hasTempoMax = conditions.TryGetValue("tempoMax", out var tempoMaxRaw) && int.TryParse(tempoMaxRaw, out tempoMax);
        if (hasTempoMin || hasTempoMax)
        {
            query = query.Where(s => int.TryParse(s.Tempo, out var bpm) && (!hasTempoMin || bpm >= tempoMin) && (!hasTempoMax || bpm <= tempoMax));
        }

        return query.OrderBy(s => s.Title).ToList();
    }

    private static List<string> ReadCsv(Dictionary<string, string> conditions, string key)
    {
        if (!conditions.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw)) return new List<string>();
        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static void EnsureCanManuallyEditSongs(Setlist existing, string userId, bool isAdmin)
    {
        if (existing.Type == SetlistTypes.Smart)
            throw new InvalidOperationException("Smart setlists are automated and do not support manual add/remove.");

        if (existing.Type == SetlistTypes.Global)
        {
            if (!isAdmin)
                throw new UnauthorizedAccessException("Only administrators can modify global setlists.");
            return;
        }

        if (existing.Type == SetlistTypes.My && existing.UserId != userId)
            throw new UnauthorizedAccessException("You can only modify your own setlists.");
    }

    private static bool MatchesSongId(BsonValue value, int songId)
    {
        if (value.IsInt32) return value.AsInt32 == songId;
        if (value.IsInt64) return value.AsInt64 == songId;
        if (value.IsString && int.TryParse(value.AsString, out var parsed)) return parsed == songId;
        if (value.IsBsonDocument && value.AsBsonDocument.TryGetValue("id", out var idValue))
        {
            if (idValue.IsInt32) return idValue.AsInt32 == songId;
            if (idValue.IsInt64) return idValue.AsInt64 == songId;
            if (idValue.IsString && int.TryParse(idValue.AsString, out var docParsed)) return docParsed == songId;
        }

        return false;
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
