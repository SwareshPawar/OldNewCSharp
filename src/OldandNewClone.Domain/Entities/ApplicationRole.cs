using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace OldandNewClone.Domain.Entities;

[CollectionName("Roles")]
public class ApplicationRole : MongoIdentityRole<string>
{
}
