using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Interfaces;

public interface IRecommendationWeightsRepository
{
    Task<RecommendationWeights> GetAsync();
    Task<RecommendationWeights?> SaveAsync(RecommendationWeights weights, DateTime? expectedLastModified = null);
}
