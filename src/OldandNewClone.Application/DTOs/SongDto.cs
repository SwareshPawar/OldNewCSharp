namespace OldandNewClone.Application.DTOs;

public record SongDto(
    int Id,
    string Title,
    string Category,
    string Key,
    string Tempo,
    string Time,
    string Taal,
    List<string> Genres,
    string Lyrics,
    string? Singer,
    string? ArtistDetails,
    string? Mood,
    List<string>? Tags,
    string? RhythmSetId,
    string? RhythmCategory,
    string? YoutubeLink,
    string? SpotifyLink,
    string? Notes,
    bool IsPublic,
    int ViewCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record SongListItemDto(
    int Id,
    string Title,
    string Category,
    string Key,
    List<string> Genres,
    string? Singer,
    string? ArtistDetails,
    string? Mood,
    string? Tempo,
    string? Time,
    string? Taal,
    List<string>? Tags
);

public record CreateSongDto(
    string Title,
    string Category,
    string Key,
    string Tempo,
    string Time,
    string Taal,
    List<string> Genres,
    string Lyrics,
    string? Singer = null,
    string? ArtistDetails = null,
    string? Mood = null,
    List<string>? Tags = null,
    string? RhythmSetId = null,
    string? RhythmCategory = null,
    string? YoutubeLink = null,
    string? SpotifyLink = null,
    string? Notes = null,
    bool IsPublic = true
);

public record UpdateSongDto(
    int Id,
    string Title,
    string Category,
    string Key,
    string Tempo,
    string Time,
    string Taal,
    List<string> Genres,
    string Lyrics,
    string? Singer = null,
    string? ArtistDetails = null,
    string? Mood = null,
    List<string>? Tags = null,
    string? RhythmSetId = null,
    string? RhythmCategory = null,
    string? YoutubeLink = null,
    string? SpotifyLink = null,
    string? Notes = null,
    bool IsPublic = true
);
