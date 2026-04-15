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
    string Lyrics
);

public record SongListItemDto(
    int Id,
    string Title,
    string Category,
    string Key,
    List<string> Genres
);

public record CreateSongDto(
    string Title,
    string Category,
    string Key,
    string Tempo,
    string Time,
    string Taal,
    List<string> Genres,
    string Lyrics
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
    string Lyrics
);
