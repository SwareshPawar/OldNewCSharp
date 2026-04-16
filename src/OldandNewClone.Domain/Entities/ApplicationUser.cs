using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace OldandNewClone.Domain.Entities;

[CollectionName("Users")]
public class ApplicationUser : MongoIdentityUser<string>
{
    // Match Node.js user structure
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;

    // Keep existing fields for compatibility
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
