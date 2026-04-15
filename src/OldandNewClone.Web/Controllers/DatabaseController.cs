using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Infrastructure.Persistence;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly MongoContext _context;
    private readonly ILogger<DatabaseController> _logger;

    public DatabaseController(MongoContext context, ILogger<DatabaseController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("health")]
    public async Task<IActionResult> CheckHealth()
    {
        try
        {
            var ping = await _context.Database.RunCommandAsync<MongoDB.Bson.BsonDocument>(new MongoDB.Bson.BsonDocument { { "ping", 1 } });

            return Ok(new 
            { 
                Status = "Connected",
                Database = _context.Database.DatabaseNamespace.DatabaseName,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return StatusCode(500, new 
            { 
                Status = "Disconnected",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("collections")]
    public async Task<IActionResult> GetCollections()
    {
        try
        {
            var collections = await _context.Database.ListCollectionNamesAsync();
            var collectionList = new List<string>();

            await foreach (var collection in collections.ToEnumerable())
            {
                collectionList.Add(collection);
            }

            return Ok(new 
            { 
                Database = _context.Database.DatabaseNamespace.DatabaseName,
                Collections = collectionList,
                Count = collectionList.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve collections");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
