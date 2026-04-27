using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OldandNewClone.Domain.Entities;

public class Song
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; } = null!;

    [BsonElement("id")]
    public int SongId { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("key")]
    public string Key { get; set; } = string.Empty;

    [BsonElement("tempo")]
    public string Tempo { get; set; } = string.Empty;

    [BsonElement("time")]
    public string Time { get; set; } = string.Empty;

    [BsonElement("taal")]
    public string Taal { get; set; } = string.Empty;

    [BsonElement("genres")]
    public List<string> Genres { get; set; } = new();

    [BsonElement("lyrics")]
    public string Lyrics { get; set; } = string.Empty;

    [BsonElement("createdBy")]
    [BsonIgnoreIfNull]
    public string? CreatedBy { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    [BsonIgnoreIfNull]
    public DateTime? UpdatedAt { get; set; }

    [BsonElement("singer")]
    [BsonIgnoreIfNull]
    public string? Singer { get; set; }

    [BsonElement("artistDetails")]
    [BsonIgnoreIfNull]
    public string? ArtistDetails { get; set; }

    [BsonElement("mood")]
    [BsonIgnoreIfNull]
    public string? Mood { get; set; }

    [BsonElement("rhythmSetId")]
    [BsonIgnoreIfNull]
    public string? RhythmSetId { get; set; }

    [BsonElement("rhythmCategory")]
    [BsonIgnoreIfNull]
    public string? RhythmCategory { get; set; }

    [BsonElement("tags")]
    [BsonIgnoreIfNull]
    public List<string>? Tags { get; set; }

    [BsonElement("youtubeLink")]
    [BsonIgnoreIfNull]
    public string? YoutubeLink { get; set; }

    [BsonElement("spotifyLink")]
    [BsonIgnoreIfNull]
    public string? SpotifyLink { get; set; }

    [BsonElement("notes")]
    [BsonIgnoreIfNull]
    public string? Notes { get; set; }

    [BsonElement("isPublic")]
    [BsonIgnoreIfDefault]
    public bool IsPublic { get; set; } = true;

    [BsonElement("viewCount")]
    [BsonIgnoreIfDefault]
    public int ViewCount { get; set; } = 0;

    // Ignore any extra fields from MongoDB that we don't have properties for
    [BsonExtraElements]
    public BsonDocument? ExtraElements { get; set; }
}
