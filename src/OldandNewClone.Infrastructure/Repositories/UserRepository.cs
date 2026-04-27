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
            var userId = NormalizeBsonId(bsonUser.GetValue("_id", BsonNull.Value));
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

    public async Task<List<ApplicationUser>> GetAllAsync()
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("Users");
            var docs = await collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();

            var users = new List<ApplicationUser>();
            foreach (var doc in docs)
            {
                var id = NormalizeBsonId(doc.GetValue("_id", BsonNull.Value));
                var uname = doc.Contains("UserName") ? doc.GetValue("UserName", BsonNull.Value).ToString()
                           : doc.GetValue("username", BsonNull.Value).ToString() ?? "";
                var email = doc.Contains("Email") ? doc.GetValue("Email", BsonNull.Value).ToString()
                           : doc.GetValue("email", BsonNull.Value).ToString() ?? "";
                var firstName = doc.Contains("FirstName") ? doc.GetValue("FirstName", BsonNull.Value).ToString() ?? ""
                               : doc.GetValue("firstName", BsonNull.Value).ToString() ?? "";
                var lastName = doc.Contains("LastName") ? doc.GetValue("LastName", BsonNull.Value).ToString() ?? ""
                              : doc.GetValue("lastName", BsonNull.Value).ToString() ?? "";
                var isAdmin = doc.Contains("IsAdmin") ? doc.GetValue("IsAdmin", false).ToBoolean()
                             : doc.Contains("isAdmin") && doc.GetValue("isAdmin", false).ToBoolean();
                var phone = doc.Contains("Phone") ? doc.GetValue("Phone", BsonNull.Value).ToString() ?? ""
                           : doc.GetValue("phone", BsonNull.Value).ToString() ?? "";

                users.Add(new ApplicationUser
                {
                    Id = id,
                    UserName = uname,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Phone = phone,
                    IsAdmin = isAdmin
                });
            }

            return users.OrderByDescending(u => u.IsAdmin)
                        .ThenBy(u => u.UserName)
                        .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all users");
            return [];
        }
    }

    public async Task<bool> SetAdminStatusAsync(string id, bool isAdmin)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("Users");
            var filter = BuildIdFilter(id);

            var update = Builders<BsonDocument>.Update
                .Set("IsAdmin", isAdmin)
                .Set("isAdmin", isAdmin); // keep both casing for Node.js compat

            var result = await collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0 || result.MatchedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting admin status for user {Id}", id);
            return false;
        }
    }

    public async Task<bool> UpdateUserProfileAsync(string id, string username, string email, string firstName, string lastName, string phone)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("Users");
            var filter = BuildIdFilter(id);

            var update = Builders<BsonDocument>.Update
                .Set("UserName", username)
                .Set("username", username)
                .Set("NormalizedUserName", username.ToUpperInvariant())
                .Set("Email", email)
                .Set("email", email)
                .Set("NormalizedEmail", email.ToUpperInvariant())
                .Set("FirstName", firstName)
                .Set("firstName", firstName)
                .Set("LastName", lastName)
                .Set("lastName", lastName)
                .Set("Phone", phone)
                .Set("phone", phone)
                .Set("Name", string.Join(" ", new[] { firstName, lastName }.Where(s => !string.IsNullOrWhiteSpace(s))));

            var result = await collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0 || result.MatchedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for user {Id}", id);
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        try
        {
            var collection = _database.GetCollection<BsonDocument>("Users");
            var normalizedId = NormalizeId(id);

            // First try direct collection delete for both ObjectId and string _id forms.
            var result = await collection.DeleteOneAsync(BuildIdFilter(id));
            if (result.DeletedCount > 0)
            {
                return true;
            }

            // Fallback to Identity store delete for records resolved by UserManager.
            var user = await _userManager.FindByIdAsync(normalizedId);
            if (user == null && !string.Equals(normalizedId, id, StringComparison.Ordinal))
            {
                user = await _userManager.FindByIdAsync(id);
            }

            if (user != null)
            {
                var identityDelete = await _userManager.DeleteAsync(user);
                if (identityDelete.Succeeded)
                {
                    return true;
                }

                var errors = string.Join(", ", identityDelete.Errors.Select(e => e.Description));
                _logger.LogWarning("Identity delete failed for user {Id}: {Errors}", id, errors);
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Id}", id);
            return false;
        }
    }

    private static FilterDefinition<BsonDocument> BuildIdFilter(string id)
    {
        var normalizedId = NormalizeId(id);

        if (ObjectId.TryParse(normalizedId, out var objectId))
        {
            return Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Eq("_id", objectId),
                Builders<BsonDocument>.Filter.Eq("_id", normalizedId),
                Builders<BsonDocument>.Filter.Eq("_id", id));
        }

        return Builders<BsonDocument>.Filter.Or(
            Builders<BsonDocument>.Filter.Eq("_id", normalizedId),
            Builders<BsonDocument>.Filter.Eq("_id", id));
    }

    private static string NormalizeBsonId(BsonValue value)
    {
        if (value.BsonType == BsonType.ObjectId)
        {
            return value.AsObjectId.ToString();
        }

        return NormalizeId(value.ToString() ?? string.Empty);
    }

    private static string NormalizeId(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return string.Empty;
        }

        const string objectIdPrefix = "ObjectId(\"";
        const string objectIdSuffix = "\")";

        if (id.StartsWith(objectIdPrefix, StringComparison.Ordinal) && id.EndsWith(objectIdSuffix, StringComparison.Ordinal))
        {
            return id.Substring(objectIdPrefix.Length, id.Length - objectIdPrefix.Length - objectIdSuffix.Length);
        }

        return id;
    }
}
