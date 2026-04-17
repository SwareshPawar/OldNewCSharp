namespace OldandNewClone.Application.DTOs;

public record SetlistDto(
    string Id,
    string Name,
    string? Description,
    string Type,
    string? OwnerUserId,
    List<int> Songs,
    Dictionary<string, string>? Conditions,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateSetlistDto(
    string Name,
    string? Description,
    string Type,
    List<int>? Songs,
    Dictionary<string, string>? Conditions,
    bool IsPublic
);

public record UpdateSetlistDto(
    string Name,
    string? Description,
    List<int>? Songs,
    Dictionary<string, string>? Conditions,
    bool IsPublic
);
