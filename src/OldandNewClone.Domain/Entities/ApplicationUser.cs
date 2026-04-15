using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace OldandNewClone.Domain.Entities;

[CollectionName("Users")]
public class ApplicationUser : MongoIdentityUser<string>
{
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
}
