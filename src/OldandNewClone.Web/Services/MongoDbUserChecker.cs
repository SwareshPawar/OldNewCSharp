using MongoDB.Driver;
using MongoDB.Bson;
using OldandNewClone.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace OldandNewClone.Web.Services;

/// <summary>
/// Direct MongoDB user checker - bypasses ASP.NET Core Identity to inspect raw user data
/// </summary>
public class MongoDbUserChecker
{
    private readonly MongoContext _mongoContext;
    private readonly ILogger<MongoDbUserChecker> _logger;

    public MongoDbUserChecker(
        MongoContext mongoContext,
        ILogger<MongoDbUserChecker> logger)
    {
        _mongoContext = mongoContext;
        _logger = logger;
    }

    public async Task<UserCheckResult> CheckUserAsync(string emailOrUsername)
    {
        try
        {
            var collection = _mongoContext.Database.GetCollection<BsonDocument>("Users");
            var loginInput = emailOrUsername.ToLower();

            // Try all possible field combinations
            var filter = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Eq("email", loginInput),
                Builders<BsonDocument>.Filter.Eq("Email", loginInput),
                Builders<BsonDocument>.Filter.Eq("username", loginInput),
                Builders<BsonDocument>.Filter.Eq("UserName", loginInput),
                Builders<BsonDocument>.Filter.Eq("NormalizedEmail", loginInput.ToUpper()),
                Builders<BsonDocument>.Filter.Eq("NormalizedUserName", loginInput.ToUpper())
            );

            var user = await collection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return new UserCheckResult
                {
                    Found = false,
                    Message = $"User not found in MongoDB with email/username: {emailOrUsername}"
                };
            }

            var result = new UserCheckResult
            {
                Found = true,
                UserId = user.GetValue("_id", BsonNull.Value).ToString(),
                Username = user.GetValue("username", user.GetValue("UserName", BsonNull.Value)).ToString(),
                Email = user.GetValue("email", user.GetValue("Email", BsonNull.Value)).ToString(),
                NormalizedUserName = user.GetValue("NormalizedUserName", BsonNull.Value).ToString(),
                NormalizedEmail = user.GetValue("NormalizedEmail", BsonNull.Value).ToString(),
                FirstName = user.GetValue("firstName", user.GetValue("FirstName", BsonNull.Value)).ToString(),
                LastName = user.GetValue("lastName", user.GetValue("LastName", BsonNull.Value)).ToString(),
                Phone = user.GetValue("phone", user.GetValue("Phone", BsonNull.Value)).ToString(),
                HasPasswordField = user.Contains("password"),
                HasPasswordHashField = user.Contains("PasswordHash"),
                PasswordValue = user.GetValue("password", BsonNull.Value).ToString(),
                PasswordHashValue = user.GetValue("PasswordHash", BsonNull.Value).ToString(),
                AllFields = user.Names.ToList()
            };

            // Check if password fields are populated
            if (!string.IsNullOrEmpty(result.PasswordValue))
            {
                result.PasswordPrefix = result.PasswordValue.Substring(0, Math.Min(20, result.PasswordValue.Length));
            }
            if (!string.IsNullOrEmpty(result.PasswordHashValue))
            {
                result.PasswordHashPrefix = result.PasswordHashValue.Substring(0, Math.Min(20, result.PasswordHashValue.Length));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user in MongoDB");
            return new UserCheckResult
            {
                Found = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    public async Task<bool> TestPasswordAsync(string emailOrUsername, string password)
    {
        try
        {
            var userCheck = await CheckUserAsync(emailOrUsername);
            if (!userCheck.Found)
            {
                _logger.LogWarning("User not found: {EmailOrUsername}", emailOrUsername);
                return false;
            }

            // Try password field first (Node.js)
            if (!string.IsNullOrEmpty(userCheck.PasswordValue))
            {
                try
                {
                    var result = BCrypt.Net.BCrypt.Verify(password, userCheck.PasswordValue);
                    _logger.LogInformation("Password test using 'password' field: {Result}", result);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error verifying with 'password' field");
                }
            }

            // Try PasswordHash field (.NET)
            if (!string.IsNullOrEmpty(userCheck.PasswordHashValue))
            {
                try
                {
                    var result = BCrypt.Net.BCrypt.Verify(password, userCheck.PasswordHashValue);
                    _logger.LogInformation("Password test using 'PasswordHash' field: {Result}", result);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error verifying with 'PasswordHash' field");
                }
            }

            _logger.LogWarning("No password fields found for user: {EmailOrUsername}", emailOrUsername);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing password");
            return false;
        }
    }
}

public class UserCheckResult
{
    public bool Found { get; set; }
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? NormalizedUserName { get; set; }
    public string? NormalizedEmail { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public bool HasPasswordField { get; set; }
    public bool HasPasswordHashField { get; set; }
    public string? PasswordValue { get; set; }
    public string? PasswordHashValue { get; set; }
    public string? PasswordPrefix { get; set; }
    public string? PasswordHashPrefix { get; set; }
    public List<string> AllFields { get; set; } = new();
    public string? Message { get; set; }
}
