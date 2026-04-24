using MongoDB.Bson;
using MongoDB.Driver;
using OldandNewClone.Infrastructure.Persistence;

namespace OldandNewClone.Web.Services;

public sealed class PasswordFieldMigration
{
    private readonly MongoContext _mongoContext;
    private readonly ILogger<PasswordFieldMigration> _logger;

    public PasswordFieldMigration(MongoContext mongoContext, ILogger<PasswordFieldMigration> logger)
    {
        _mongoContext = mongoContext;
        _logger = logger;
    }

    public async Task<PasswordMigrationStatus> GetStatusAsync()
    {
        var users = _mongoContext.Database.GetCollection<BsonDocument>("Users");

        var totalUsers = await users.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);
        var usersWithPasswordField = await users.CountDocumentsAsync(Builders<BsonDocument>.Filter.Exists("password", true));
        var usersWithPasswordHashField = await users.CountDocumentsAsync(Builders<BsonDocument>.Filter.Exists("PasswordHash", true));

        var usersNeedingMigration = await users.CountDocumentsAsync(
            Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Exists("password", true),
                Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Exists("PasswordHash", false),
                    Builders<BsonDocument>.Filter.Eq("PasswordHash", BsonNull.Value),
                    Builders<BsonDocument>.Filter.Eq("PasswordHash", string.Empty)
                )
            )
        );

        return new PasswordMigrationStatus
        {
            TotalUsers = totalUsers,
            UsersWithPasswordField = usersWithPasswordField,
            UsersWithPasswordHashField = usersWithPasswordHashField,
            UsersNeedingMigration = usersNeedingMigration,
            MigrationNeeded = usersNeedingMigration > 0
        };
    }

    public async Task<PasswordMigrationResult> MigratePasswordFieldsAsync()
    {
        var users = _mongoContext.Database.GetCollection<BsonDocument>("Users");
        var status = await GetStatusAsync();

        if (status.UsersNeedingMigration == 0)
        {
            _logger.LogInformation("Password migration skipped: no users need migration.");
            return new PasswordMigrationResult
            {
                ScannedUsers = status.TotalUsers,
                MigratedUsers = 0,
                FailedUsers = 0,
                Status = await GetStatusAsync()
            };
        }

        _logger.LogInformation("Password migration started. Users needing migration: {UsersNeedingMigration}", status.UsersNeedingMigration);

        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Exists("password", true),
            Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Exists("PasswordHash", false),
                Builders<BsonDocument>.Filter.Eq("PasswordHash", BsonNull.Value),
                Builders<BsonDocument>.Filter.Eq("PasswordHash", string.Empty)
            )
        );

        var candidates = await users.Find(filter).ToListAsync();
        long migrated = 0;
        long failed = 0;

        foreach (var candidate in candidates)
        {
            var id = candidate.GetValue("_id", BsonNull.Value);
            var username = candidate.GetValue("username", BsonNull.Value).ToString();
            var password = candidate.GetValue("password", BsonNull.Value).ToString();

            if (string.IsNullOrWhiteSpace(password))
            {
                failed++;
                _logger.LogWarning("Skipping migration for user {Username} ({UserId}) due to empty password field.", username, id);
                continue;
            }

            var update = Builders<BsonDocument>.Update.Set("PasswordHash", password);
            var result = await users.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", id), update);

            if (result.ModifiedCount == 1)
            {
                migrated++;
                _logger.LogInformation("Migrated PasswordHash for user {Username} ({UserId}).", username, id);
            }
            else
            {
                failed++;
                _logger.LogWarning("No document updated for user {Username} ({UserId}).", username, id);
            }
        }

        var finalStatus = await GetStatusAsync();
        _logger.LogInformation(
            "Password migration complete. Scanned: {Scanned}, Migrated: {Migrated}, Failed: {Failed}, Remaining: {Remaining}",
            status.TotalUsers,
            migrated,
            failed,
            finalStatus.UsersNeedingMigration);

        return new PasswordMigrationResult
        {
            ScannedUsers = status.TotalUsers,
            MigratedUsers = migrated,
            FailedUsers = failed,
            Status = finalStatus
        };
    }
}

public sealed class PasswordMigrationStatus
{
    public long TotalUsers { get; set; }
    public long UsersWithPasswordField { get; set; }
    public long UsersWithPasswordHashField { get; set; }
    public long UsersNeedingMigration { get; set; }
    public bool MigrationNeeded { get; set; }
}

public sealed class PasswordMigrationResult
{
    public long ScannedUsers { get; set; }
    public long MigratedUsers { get; set; }
    public long FailedUsers { get; set; }
    public PasswordMigrationStatus Status { get; set; } = new();
}