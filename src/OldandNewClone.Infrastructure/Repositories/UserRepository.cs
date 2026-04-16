using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMongoDatabase _database;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        UserManager<ApplicationUser> userManager,
        IMongoDatabase database,
        ILogger<UserRepository> logger)
    {
        _userManager = userManager;
        _database = database;
        _logger = logger;
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<ApplicationUser?> GetByUsernameOrEmailAsync(string loginInput)
    {
        // Try username first
        var user = await _userManager.FindByNameAsync(loginInput);
        if (user != null) return user;

        // Try email
        return await _userManager.FindByEmailAsync(loginInput);
    }

    public async Task<ApplicationUser?> GetByUsernameOrEmailHybridAsync(string loginInput)
    {
        var normalizedInput = loginInput.ToLower();

        // Try UserManager first (.NET users)
        var user = await GetByUsernameOrEmailAsync(normalizedInput);
        if (user != null) return user;

        // If not found, try direct MongoDB (Node.js users)
        return await FindNodeJsUserAsync(normalizedInput);
    }

    private async Task<ApplicationUser?> FindNodeJsUserAsync(string loginInput)
    {
        try
        {
            if (_database == null)
            {
                _logger.LogError("MongoDB database is null!");
                return null;
            }

            var collection = _database.GetCollection<BsonDocument>("Users");

            var filter = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Eq("username", loginInput),
                Builders<BsonDocument>.Filter.Eq("email", loginInput)
            );

            _logger.LogDebug("Searching MongoDB for user with login input: {LoginInput}", loginInput);

            var bsonUser = await collection.Find(filter).FirstOrDefaultAsync();

            if (bsonUser == null)
            {
                _logger.LogDebug("No Node.js user found in MongoDB for: {LoginInput}", loginInput);
                return null;
            }

            _logger.LogInformation("Found Node.js user in MongoDB for: {LoginInput}", loginInput);

            // Convert Node.js user to ApplicationUser
            var userId = bsonUser.GetValue("_id", BsonNull.Value).ToString();
            var username = bsonUser.GetValue("username", BsonNull.Value).ToString();
            var email = bsonUser.GetValue("email", BsonNull.Value).ToString();

            var user = new ApplicationUser
            {
                Id = userId,
                UserName = username,
                Email = email,
                NormalizedUserName = username?.ToUpper(),
                NormalizedEmail = email?.ToUpper(),
                FirstName = bsonUser.GetValue("firstName", BsonNull.Value).ToString() ?? "",
                LastName = bsonUser.GetValue("lastName", BsonNull.Value).ToString() ?? "",
                Phone = bsonUser.GetValue("phone", BsonNull.Value).ToString() ?? "",
                IsAdmin = bsonUser.GetValue("isAdmin", false).AsBoolean,
                Name = $"{bsonUser.GetValue("firstName", BsonNull.Value)} {bsonUser.GetValue("lastName", BsonNull.Value)}",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                // Required by ASP.NET Core Identity
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            // Copy password field to PasswordHash
            if (bsonUser.Contains("password"))
            {
                var passwordHash = bsonUser.GetValue("password").ToString();
                user.PasswordHash = passwordHash;
                _logger.LogInformation("Found Node.js user {Username} (ID: {UserId}), copied password field to PasswordHash", username, userId);
            }
            else if (bsonUser.Contains("PasswordHash"))
            {
                user.PasswordHash = bsonUser.GetValue("PasswordHash").ToString();
                _logger.LogInformation("Found Node.js user {Username} with PasswordHash field", username);
            }
            else
            {
                _logger.LogWarning("Node.js user {Username} has no password or PasswordHash field!", username);
            }

            return user;
        }
        catch (MongoDB.Driver.MongoException mex)
        {
            _logger.LogError(mex, "MongoDB error finding Node.js user for: {LoginInput}", loginInput);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding Node.js user for: {LoginInput}. Type: {ExceptionType}", loginInput, ex.GetType().Name);
            return null;
        }
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<ApplicationUser> CreateAsync(ApplicationUser user)
    {
        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }
        return user;
    }

    public async Task<bool> UpdateAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        return user != null;
    }
}
