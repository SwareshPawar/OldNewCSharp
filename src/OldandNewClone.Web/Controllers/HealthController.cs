using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OldandNewClone.Infrastructure.Persistence;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly MongoContext _mongoContext;

    public HealthController(MongoContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var health = new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow,
            version = "1.0.0",
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        };

        return Ok(health);
    }

    [HttpGet("database")]
    public async Task<IActionResult> CheckDatabase()
    {
        try
        {
            // Ping MongoDB
            var database = _mongoContext.Database;
            await database.RunCommandAsync((Command<MongoDB.Bson.BsonDocument>)"{ping:1}");

            var songCount = await _mongoContext.Songs.CountDocumentsAsync(FilterDefinition<Domain.Entities.Song>.Empty);

            return Ok(new
            {
                status = "Connected",
                database = database.DatabaseNamespace.DatabaseName,
                songCount = songCount,
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = "Unhealthy",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
