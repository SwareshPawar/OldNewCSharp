using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/recommendation-weights")]
public class RecommendationWeightsController : ControllerBase
{
    private readonly IRecommendationWeightsRepository _repo;
    private readonly ILogger<RecommendationWeightsController> _logger;

    public RecommendationWeightsController(IRecommendationWeightsRepository repo, ILogger<RecommendationWeightsController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
        try
        {
            var weights = await _repo.GetAsync();
            return Ok(new
            {
                language = weights.Language,
                scale = weights.Scale,
                timeSignature = weights.TimeSignature,
                taal = weights.Taal,
                tempo = weights.Tempo,
                genre = weights.Genre,
                vocal = weights.Vocal,
                mood = weights.Mood,
                rhythmCategory = weights.RhythmCategory,
                lastModified = weights.LastModified
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load recommendation weights");
            return StatusCode(500, new { error = "Failed to load recommendation weights" });
        }
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Save([FromBody] RecommendationWeightsDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var total = dto.Language + dto.Scale + dto.TimeSignature + dto.Taal
                      + dto.Tempo + dto.Genre + dto.Vocal + dto.Mood + dto.RhythmCategory;
            if (total != 100)
                return BadRequest(new { error = $"Total must be 100, got {total}" });

            var entity = new RecommendationWeights
            {
                Language = dto.Language,
                Scale = dto.Scale,
                TimeSignature = dto.TimeSignature,
                Taal = dto.Taal,
                Tempo = dto.Tempo,
                Genre = dto.Genre,
                Vocal = dto.Vocal,
                Mood = dto.Mood,
                RhythmCategory = dto.RhythmCategory
            };

            var saved = await _repo.SaveAsync(entity, dto.ExpectedLastModified);
            if (saved == null)
                return Conflict(new { error = "Weights were updated by someone else. Refresh and try again." });

            return Ok(new { message = "Weights updated", lastModified = saved.LastModified });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save recommendation weights");
            return StatusCode(500, new { error = "Failed to save recommendation weights" });
        }
    }
}

public record RecommendationWeightsDto(
    [Range(0, 100)] int Language,
    [Range(0, 100)] int Scale,
    [Range(0, 100)] int TimeSignature,
    [Range(0, 100)] int Taal,
    [Range(0, 100)] int Tempo,
    [Range(0, 100)] int Genre,
    [Range(0, 100)] int Vocal,
    [Range(0, 100)] int Mood,
    [Range(0, 100)] int RhythmCategory,
    DateTime? ExpectedLastModified = null);
