using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace OldandNewClone.Domain.Entities;

[CollectionName("recommendationWeights")]
public class RecommendationWeights
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    // Singleton document identifier
    public string Key { get; set; } = "global";

    public int Language { get; set; } = 15;
    public int Scale { get; set; } = 10;
    public int TimeSignature { get; set; } = 10;
    public int Taal { get; set; } = 15;
    public int Tempo { get; set; } = 10;
    public int Genre { get; set; } = 15;
    public int Vocal { get; set; } = 10;
    public int Mood { get; set; } = 10;
    public int RhythmCategory { get; set; } = 5;

    public DateTime LastModified { get; set; } = DateTime.UtcNow;
}
