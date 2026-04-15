namespace OldandNewClone.Domain.Entities;

public class UserData
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public List<int> Favorites { get; set; } = new();
    public List<int> NewSetlist { get; set; } = new();
    public List<int> OldSetlist { get; set; } = new();
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
