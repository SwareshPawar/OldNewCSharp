using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OldandNewClone.Domain.Entities;

[BsonIgnoreExtraElements]
public class Setlist
{
    [BsonId]
    public BsonValue? RawId { get; set; }

    [BsonIgnore]
    public string Id => RawId switch
    {
        BsonObjectId oid => oid.Value.ToString(),
        BsonString s => s.Value,
        null => string.Empty,
        _ => RawId.ToString()
    };

    [BsonIgnore]
    public string Type { get; set; } = SetlistTypes.My;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("description")]
    [BsonIgnoreIfNull]
    public string? Description { get; set; }

    [BsonElement("userId")]
    [BsonIgnoreIfNull]
    public string? UserId { get; set; }

    [BsonElement("createdBy")]
    [BsonIgnoreIfNull]
    public string? CreatedBy { get; set; }

    [BsonElement("createdByUsername")]
    [BsonIgnoreIfNull]
    public string? CreatedByUsername { get; set; }

    [BsonElement("isAdminCreated")]
    [BsonIgnoreIfNull]
    public bool? IsAdminCreated { get; set; }

    [BsonElement("songs")]
    public List<BsonValue> SongsRaw { get; set; } = new();

    [BsonElement("conditions")]
    [BsonIgnoreIfNull]
    public BsonDocument? ConditionsRaw { get; set; }

    [BsonElement("createdAt")]
    [BsonIgnoreIfNull]
    public BsonValue? CreatedAtRaw { get; set; }

    [BsonElement("updatedAt")]
    [BsonIgnoreIfNull]
    public BsonValue? UpdatedAtRaw { get; set; }

    [BsonIgnore]
    public List<int> SongIds => SongsRaw
        .Select(value =>
        {
            if (value == null) return (int?)null;
            if (value.IsInt32) return value.AsInt32;
            if (value.IsInt64) return (int)value.AsInt64;
            if (value.IsString && int.TryParse(value.AsString, out var parsed)) return parsed;
            if (value.IsBsonDocument)
            {
                var doc = value.AsBsonDocument;
                if (doc.TryGetValue("id", out var idValue))
                {
                    if (idValue.IsInt32) return idValue.AsInt32;
                    if (idValue.IsInt64) return (int)idValue.AsInt64;
                    if (idValue.IsString && int.TryParse(idValue.AsString, out var docParsed)) return docParsed;
                }
            }
            return (int?)null;
        })
        .Where(id => id.HasValue)
        .Select(id => id!.Value)
        .ToList();

    [BsonIgnore]
    public Dictionary<string, string>? Conditions =>
        ConditionsRaw?.Elements.ToDictionary(e => e.Name, e => e.Value.ToString());

    [BsonIgnore]
    public DateTime CreatedAt => ParseDate(CreatedAtRaw);

    [BsonIgnore]
    public DateTime UpdatedAt => ParseDate(UpdatedAtRaw);

    private static DateTime ParseDate(BsonValue? value)
    {
        if (value == null || value.IsBsonNull) return DateTime.UtcNow;
        if (value.IsValidDateTime) return value.ToUniversalTime();
        if (value.IsString && DateTime.TryParse(value.AsString, out var parsed)) return parsed;
        return DateTime.UtcNow;
    }
}

public static class SetlistTypes
{
    public const string Global = "global";
    public const string My = "my";
    public const string Smart = "smart";
}
