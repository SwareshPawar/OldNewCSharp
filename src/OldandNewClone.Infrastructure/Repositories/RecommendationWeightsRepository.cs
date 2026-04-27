using MongoDB.Driver;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Infrastructure.Repositories;

public class RecommendationWeightsRepository : IRecommendationWeightsRepository
{
    private readonly IMongoCollection<RecommendationWeights> _collection;

    public RecommendationWeightsRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<RecommendationWeights>("recommendationWeights");
    }

    public async Task<RecommendationWeights> GetAsync()
    {
        var weights = await _collection.Find(w => w.Key == "global").FirstOrDefaultAsync();
        return weights ?? new RecommendationWeights();
    }

    public async Task<RecommendationWeights?> SaveAsync(RecommendationWeights weights, DateTime? expectedLastModified = null)
    {
        weights.Key = "global";
        weights.LastModified = DateTime.UtcNow;

        var filter = Builders<RecommendationWeights>.Filter.Eq(w => w.Key, "global");
        if (expectedLastModified.HasValue)
        {
            filter = Builders<RecommendationWeights>.Filter.And(
                filter,
                Builders<RecommendationWeights>.Filter.Eq(w => w.LastModified, expectedLastModified.Value));
        }

        var options = new FindOneAndReplaceOptions<RecommendationWeights>
        {
            IsUpsert = !expectedLastModified.HasValue,
            ReturnDocument = ReturnDocument.After
        };
        var result = await _collection.FindOneAndReplaceAsync(filter, weights, options);
        return result;
    }
}
