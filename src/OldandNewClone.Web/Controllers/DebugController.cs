using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OldandNewClone.Domain.Entities;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Web.Services;
using MongoDB.Driver;
using MongoDB.Bson;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly MongoDbUserChecker _userChecker;
    private readonly IMongoDatabase _database;
    private readonly ILogger<DebugController> _logger;

    public DebugController(
        UserManager<ApplicationUser> userManager,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IUserRepository userRepository,
        MongoDbUserChecker userChecker,
        IMongoDatabase database,
        ILogger<DebugController> logger)
    {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _userChecker = userChecker;
        _database = database;
        _logger = logger;
    }

    [HttpGet("users")]
    public async Task<IActionResult> ListUsers()
    {
        try
        {
            // Get all users directly from MongoDB to include Node.js users
            var collection = _database.GetCollection<BsonDocument>("Users");
            var allUsers = await collection.Find(new BsonDocument()).ToListAsync();

            var userList = allUsers.Select(u => new
            {
                Id = u.GetValue("_id", BsonNull.Value).ToString(),
                Email = u.GetValue("email", u.GetValue("Email", BsonNull.Value)).ToString(),
                UserName = u.GetValue("username", u.GetValue("UserName", BsonNull.Value)).ToString(),
                FirstName = u.GetValue("firstName", u.GetValue("FirstName", BsonNull.Value)).ToString(),
                LastName = u.GetValue("lastName", u.GetValue("LastName", BsonNull.Value)).ToString(),
                HasPassword = u.Contains("password") && !string.IsNullOrEmpty(u.GetValue("password", BsonNull.Value).ToString()),
                HasPasswordHash = u.Contains("PasswordHash") && !string.IsNullOrEmpty(u.GetValue("PasswordHash", BsonNull.Value).ToString()),
                PasswordHashFormat = u.Contains("password") 
                    ? u.GetValue("password", "").ToString()?.Substring(0, Math.Min(10, u.GetValue("password", "").ToString()?.Length ?? 0))
                    : u.Contains("PasswordHash")
                        ? u.GetValue("PasswordHash", "").ToString()?.Substring(0, Math.Min(10, u.GetValue("PasswordHash", "").ToString()?.Length ?? 0))
                        : "",
                IsBcryptHash = (u.Contains("password") && u.GetValue("password", "").ToString()?.StartsWith("$2") == true) ||
                               (u.Contains("PasswordHash") && u.GetValue("PasswordHash", "").ToString()?.StartsWith("$2") == true),
                IsNodeJsUser = u.Contains("password") && !u.Contains("PasswordHash"),
                IsDotNetUser = u.Contains("PasswordHash")
            }).ToList();

            return Ok(userList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing users");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("test-password")]
    public async Task<IActionResult> TestPassword([FromBody] TestPasswordRequest request)
    {
        try
        {
            // Use hybrid lookup to find both Node.js and .NET users
            var user = await _userRepository.GetByUsernameOrEmailHybridAsync(request.Email.ToLower());

            if (user == null)
            {
                _logger.LogWarning("User not found for test-password: {Email}", request.Email);
                return NotFound(new { message = "User not found" });
            }

            if (string.IsNullOrEmpty(user.PasswordHash))
            {
                return BadRequest(new { message = "User has no password hash" });
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

            return Ok(new
            {
                email = user.Email,
                userName = user.UserName,
                passwordMatch = result == PasswordVerificationResult.Success,
                result = result.ToString(),
                storedHash = user.PasswordHash.Substring(0, Math.Min(20, user.PasswordHash.Length)),
                isBcryptHash = user.PasswordHash.StartsWith("$2")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing password");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("rehash-password")]
    public async Task<IActionResult> RehashPassword([FromBody] RehashPasswordRequest request)
    {
        try
        {
            // Use hybrid lookup
            var user = await _userRepository.GetByUsernameOrEmailHybridAsync(request.Email.ToLower());

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Hash the new password with BCrypt
            var newHash = _passwordHasher.HashPassword(user, request.NewPassword);

            user.PasswordHash = newHash;
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("Password rehashed for user: {Email}", request.Email);

            return Ok(new
            {
                message = "Password rehashed successfully",
                email = user.Email,
                newHashFormat = newHash.Substring(0, Math.Min(20, newHash.Length)),
                isBcryptHash = newHash.StartsWith("$2")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rehashing password");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("user/{email}")]
    public async Task<IActionResult> DeleteUser(string email)
    {
        try
        {
            // Try to find via hybrid lookup first
            var user = await _userRepository.GetByUsernameOrEmailHybridAsync(email.ToLower());

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Delete directly from MongoDB to handle both Node.js and .NET users
            var collection = _database.GetCollection<BsonDocument>("Users");
            var filter = Builders<BsonDocument>.Filter.Eq("_id", user.Id);
            var deleteResult = await collection.DeleteOneAsync(filter);

            if (deleteResult.DeletedCount > 0)
            {
                _logger.LogInformation("User deleted: {Email}", email);
                return Ok(new { message = "User deleted successfully", email });
            }

            return BadRequest(new { message = "Failed to delete user" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("smart-setlists/by-name/{name}")]
    public async Task<IActionResult> GetSmartSetlistsByName(string name)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("SmartSetlists");
            var filter = Builders<BsonDocument>.Filter.Eq("name", name);
            var docs = await collection.Find(filter).ToListAsync();

            return Ok(new
            {
                Name = name,
                Count = docs.Count,
                Ids = docs.Select(d => d.GetValue("_id", BsonNull.Value).ToString()).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading smart setlists by name {Name}", name);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("smart-setlists/by-name/{name}")]
    public async Task<IActionResult> DeleteSmartSetlistsByName(string name)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("SmartSetlists");
            var filter = Builders<BsonDocument>.Filter.Eq("name", name);
            var existing = await collection.Find(filter).ToListAsync();
            var deleteResult = await collection.DeleteManyAsync(filter);

            return Ok(new
            {
                Name = name,
                Found = existing.Count,
                Deleted = deleteResult.DeletedCount,
                DeletedIds = existing.Select(d => d.GetValue("_id", BsonNull.Value).ToString()).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting smart setlists by name {Name}", name);
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok("Debug endpoint is healthy");
    }

    public class TestPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RehashPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
