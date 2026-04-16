using OldandNewClone.Domain.Entities;

namespace OldandNewClone.Application.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<ApplicationUser?> GetByUsernameAsync(string username);
    Task<ApplicationUser?> GetByUsernameOrEmailAsync(string loginInput);
    Task<ApplicationUser?> GetByUsernameOrEmailHybridAsync(string loginInput); // For Node.js compatibility
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<ApplicationUser> CreateAsync(ApplicationUser user);
    Task<bool> UpdateAsync(ApplicationUser user);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
}
