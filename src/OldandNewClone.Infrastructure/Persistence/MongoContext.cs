using MongoDB.Driver;
using OldandNewClone.Domain.Entities;
using OldandNewClone.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace OldandNewClone.Infrastructure.Persistence;

public class MongoContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoContext(IOptions<MongoDbSettings> settings)
    {
        _settings = settings.Value;
        var client = new MongoClient(_settings.ConnectionString);
        _database = client.GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<Song> Songs => 
        _database.GetCollection<Song>(_settings.SongsCollectionName);

    public IMongoCollection<UserData> UserData => 
        _database.GetCollection<UserData>(_settings.UserDataCollectionName);

    public IMongoCollection<Setlist> GlobalSetlists =>
        _database.GetCollection<Setlist>("GlobalSetlists");

    public IMongoCollection<Setlist> MySetlists =>
        _database.GetCollection<Setlist>("MySetlists");

    public IMongoCollection<Setlist> SmartSetlists =>
        _database.GetCollection<Setlist>("SmartSetlists");

    public IMongoDatabase Database => _database;
}
