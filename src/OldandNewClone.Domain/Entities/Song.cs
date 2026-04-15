namespace OldandNewClone.Domain.Entities;

public class Song
{
    public string Id { get; set; } = null!;
    public int SongId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string Tempo { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Taal { get; set; } = string.Empty;
    public List<string> Genres { get; set; } = new();
    public string Lyrics { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
