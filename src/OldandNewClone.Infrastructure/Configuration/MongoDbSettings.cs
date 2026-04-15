namespace OldandNewClone.Infrastructure.Configuration;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string SongsCollectionName { get; set; } = "OldNewSongs";
    public string UserDataCollectionName { get; set; } = "UserData";
}
