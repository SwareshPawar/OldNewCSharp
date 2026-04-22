using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OldandNewClone.Domain.Entities;

[BsonIgnoreExtraElements]
public class UserData
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("email")]
    public string? Email { get; set; }

    [BsonElement("favorites")]
    public List<int> Favorites { get; set; } = new();

    [BsonElement("NewSetlist")]
    public List<int> NewSetlist { get; set; } = new();

    [BsonElement("OldSetlist")]
    public List<int> OldSetlist { get; set; } = new();

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
