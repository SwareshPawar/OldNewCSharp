using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using OldandNewClone.Domain.Entities;
using OldandNewClone.Infrastructure.Persistence;
using MongoDB.Driver;
using MongoDB.Bson;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PasswordTestController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly MongoContext _mongoContext;
    private readonly ILogger<PasswordTestController> _logger;

    public PasswordTestController(
        UserManager<ApplicationUser> userManager,
        IPasswordHasher<ApplicationUser> passwordHasher,
        MongoContext mongoContext,
        ILogger<PasswordTestController> logger)
    {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _mongoContext = mongoContext;
        _logger = logger;
    }

    [HttpGet("check-user/{usernameOrEmail}")]
    public async Task<IActionResult> CheckUser(string usernameOrEmail)
    {
        try
        {
            var loginInput = usernameOrEmail.ToLower();

            // Try to find user via UserManager
            var userViaUsername = await _userManager.FindByNameAsync(loginInput);
            var userViaEmail = await _userManager.FindByEmailAsync(loginInput);

            var user = userViaUsername ?? userViaEmail;

            if (user == null)
            {
                // Try direct MongoDB query
                var collection = _mongoContext.Database.GetCollection<BsonDocument>("Users");
                var filter = Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("username", loginInput),
                    Builders<BsonDocument>.Filter.Eq("UserName", loginInput),
                    Builders<BsonDocument>.Filter.Eq("email", loginInput),
                    Builders<BsonDocument>.Filter.Eq("Email", loginInput)
                );
                var bsonUser = await collection.Find(filter).FirstOrDefaultAsync();

                if (bsonUser != null)
                {
                    return Ok(new
                    {
                        found = "MongoDB only",
                        id = bsonUser.GetValue("_id", BsonNull.Value).ToString(),
                        username = bsonUser.GetValue("username", BsonNull.Value).ToString(),
                        email = bsonUser.GetValue("email", BsonNull.Value).ToString(),
                        UserName = bsonUser.GetValue("UserName", BsonNull.Value).ToString(),
                        Email = bsonUser.GetValue("Email", BsonNull.Value).ToString(),
                        hasPassword = bsonUser.Contains("password"),
                        hasPasswordHash = bsonUser.Contains("PasswordHash"),
                        passwordField = bsonUser.GetValue("password", BsonNull.Value).ToString(),
                        passwordHashField = bsonUser.GetValue("PasswordHash", BsonNull.Value).ToString(),
                        allFields = bsonUser.Names.ToList()
                    });
                }

                return NotFound(new { error = "User not found in UserManager or MongoDB" });
            }

            return Ok(new
            {
                found = "UserManager",
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                phone = user.Phone,
                isAdmin = user.IsAdmin,
                hasPasswordHash = !string.IsNullOrEmpty(user.PasswordHash),
                passwordHashLength = user.PasswordHash?.Length ?? 0,
                passwordHashPrefix = user.PasswordHash?.Substring(0, Math.Min(10, user.PasswordHash?.Length ?? 0))
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user");
            return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
        }
    }

    [HttpPost("test-password")]
    public async Task<IActionResult> TestPassword([FromBody] PasswordTestRequest request)
    {
        try
        {
            var loginInput = request.UsernameOrEmail.ToLower();

            // Find user
            var user = await _userManager.FindByNameAsync(loginInput);
            if (user == null)
                user = await _userManager.FindByEmailAsync(loginInput);

            if (user == null)
            {
                // Try direct MongoDB
                var collection = _mongoContext.Database.GetCollection<BsonDocument>("Users");
                var filter = Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("username", loginInput),
                    Builders<BsonDocument>.Filter.Eq("email", loginInput)
                );
                var bsonUser = await collection.Find(filter).FirstOrDefaultAsync();

                if (bsonUser != null)
                {
                    var passwordFromMongo = bsonUser.GetValue("password", BsonNull.Value).ToString();

                    // Test with BCrypt.Net directly
                    var bcryptResult = BCrypt.Net.BCrypt.Verify(request.Password, passwordFromMongo);

                    return Ok(new
                    {
                        method = "Direct MongoDB + BCrypt.Net",
                        passwordHash = passwordFromMongo,
                        hashPrefix = passwordFromMongo.Substring(0, Math.Min(20, passwordFromMongo.Length)),
                        bcryptNetVerify = bcryptResult,
                        message = bcryptResult ? "✅ Password matches!" : "❌ Password does not match"
                    });
                }

                return NotFound(new { error = "User not found" });
            }

            // Test with IPasswordHasher
            var hasherResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);

            // Also test with BCrypt.Net directly
            var directBcryptResult = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash!);

            return Ok(new
            {
                method = "UserManager",
                userId = user.Id,
                username = user.UserName,
                email = user.Email,
                passwordHashLength = user.PasswordHash?.Length ?? 0,
                passwordHashPrefix = user.PasswordHash?.Substring(0, Math.Min(20, user.PasswordHash?.Length ?? 0)),
                iPasswordHasherResult = hasherResult.ToString(),
                bcryptNetDirectResult = directBcryptResult,
                bothMatch = hasherResult == PasswordVerificationResult.Success && directBcryptResult,
                message = hasherResult == PasswordVerificationResult.Success ? "✅ Password matches!" : "❌ Password does not match"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing password");
            return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
        }
    }

    [HttpGet("bcrypt-info")]
    public IActionResult GetBcryptInfo()
    {
        // Test BCrypt.Net configuration
        var testPassword = "test123";
        var hash = BCrypt.Net.BCrypt.HashPassword(testPassword, 10);
        var verify = BCrypt.Net.BCrypt.Verify(testPassword, hash);

        return Ok(new
        {
            bcryptNetVersion = typeof(BCrypt.Net.BCrypt).Assembly.GetName().Version?.ToString(),
            testHash = hash,
            testVerify = verify,
            hashPrefix = hash.Substring(0, 10),
            hashLength = hash.Length,
            workFactor = BCrypt.Net.BCrypt.GenerateSalt(10).Substring(4, 2),
            note = "BCrypt hashes start with $2a$, $2b$, or $2y$"
        });
    }
}

public class PasswordTestRequest
{
    public string UsernameOrEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
