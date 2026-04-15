namespace OldandNewClone.Application.DTOs;

public record UserDataDto(
    string UserId,
    string? Name,
    string? Email,
    List<int> Favorites,
    List<int> NewSetlist,
    List<int> OldSetlist
);

public record UpdateUserDataDto(
    List<int> Favorites,
    List<int> NewSetlist,
    List<int> OldSetlist,
    string? Name,
    string? Email
);
