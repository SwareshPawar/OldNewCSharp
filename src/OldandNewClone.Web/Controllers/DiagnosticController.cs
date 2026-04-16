using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using OldandNewClone.Infrastructure.Persistence;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticController : ControllerBase
{
    private readonly MongoContext _context;
    private readonly ILogger<DiagnosticController> _logger;

    public DiagnosticController(MongoContext context, ILogger<DiagnosticController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("users-raw")]
    public async Task<IActionResult> GetUsersRaw()
    {
        try
        {
            var usersCollection = _context.Database.GetCollection<BsonDocument>("Users");
            var users = await usersCollection.Find(new BsonDocument()).ToListAsync();

            var userList = users.Select(u => new
            {
                RawId = u.GetValue("_id", BsonNull.Value).ToString(),
                IdType = u.GetValue("_id", BsonNull.Value).BsonType.ToString(),
                Email = u.GetValue("Email", BsonNull.Value).ToString(),
                PasswordHash = u.Contains("PasswordHash") 
                    ? u.GetValue("PasswordHash").AsString?.Substring(0, Math.Min(50, u.GetValue("PasswordHash").AsString?.Length ?? 0))
                    : "No PasswordHash",
                AllFields = u.Names.ToList()
            }).ToList();

            return Ok(new
            {
                totalUsers = users.Count,
                users = userList
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting raw users");
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpPost("test-bcrypt")]
    public IActionResult TestBCrypt([FromBody] TestBCryptRequest request)
    {
        try
        {
            // Test BCrypt hashing
            var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var isValid = BCrypt.Net.BCrypt.Verify(request.Password, hash);

            // Test against stored hash if provided
            bool? storedHashValid = null;
            if (!string.IsNullOrEmpty(request.StoredHash))
            {
                try
                {
                    storedHashValid = BCrypt.Net.BCrypt.Verify(request.Password, request.StoredHash);
                }
                catch (Exception ex)
                {
                    return Ok(new
                    {
                        newHash = hash,
                        newHashWorks = isValid,
                        storedHashValid = false,
                        storedHashError = ex.Message
                    });
                }
            }

            return Ok(new
            {
                password = request.Password,
                newHash = hash,
                newHashWorks = isValid,
                storedHash = request.StoredHash,
                storedHashValid = storedHashValid
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    public class TestBCryptRequest
    {
        public string Password { get; set; } = string.Empty;
        public string? StoredHash { get; set; }
    }
}
