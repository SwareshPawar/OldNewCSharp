using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Domain.Entities;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private static readonly SemaphoreSlim AdminMutationLock = new(1, 1);
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserRepository userRepository, UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var users = await _userRepository.GetAllAsync();
            var result = users.Select(u => new
            {
                id = u.Id,
                username = u.UserName,
                email = u.Email,
                firstName = u.FirstName,
                lastName = u.LastName,
                phone = u.Phone,
                isAdmin = u.IsAdmin
            });
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load users list");
            return StatusCode(500, new { error = "Failed to load users" });
        }
    }

    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateProfile(string id, [FromBody] UpdateUserProfileRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User id is required" });

        var username = request.Username?.Trim() ?? string.Empty;
        var email = request.Email?.Trim() ?? string.Empty;
        var firstName = request.FirstName?.Trim() ?? string.Empty;
        var lastName = request.LastName?.Trim() ?? string.Empty;
        var phone = request.Phone?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            return BadRequest(new { error = "Username, email, first name and last name are required" });

        if (username.Length > 64)
            return BadRequest(new { error = "Username must be 64 characters or fewer" });

        if (!new EmailAddressAttribute().IsValid(email))
            return BadRequest(new { error = "Email format is invalid" });

        if (firstName.Length > 60 || lastName.Length > 60)
            return BadRequest(new { error = "First name/last name must be 60 characters or fewer" });

        if (!string.IsNullOrEmpty(phone) && !Regex.IsMatch(phone, @"^[0-9+()\-\s]{7,20}$"))
            return BadRequest(new { error = "Phone number format is invalid" });

        try
        {
            var allUsers = await _userRepository.GetAllAsync();
            var normalizedId = NormalizeUserId(id);
            var duplicateUsername = allUsers.Any(u =>
                NormalizeUserId(u.Id) != normalizedId &&
                string.Equals(u.UserName, username, StringComparison.OrdinalIgnoreCase));
            if (duplicateUsername)
                return Conflict(new { error = "Username is already in use" });

            var duplicateEmail = allUsers.Any(u =>
                NormalizeUserId(u.Id) != normalizedId &&
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
            if (duplicateEmail)
                return Conflict(new { error = "Email is already in use" });

            var success = await _userRepository.UpdateUserProfileAsync(
                id,
                username,
                email,
                firstName,
                lastName,
                phone);

            if (!success) return NotFound(new { error = "User not found or update failed" });
            return Ok(new { message = "User details updated" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update profile for user {UserId}", id);
            return StatusCode(500, new { error = "Failed to update user profile" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User id is required" });

        await AdminMutationLock.WaitAsync();
        try
        {
            var currentUserId = NormalizeUserId(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var targetUserId = NormalizeUserId(id);

            if (currentUserId == targetUserId)
                return BadRequest(new { error = "Cannot delete your own account" });

            var targetUser = await FindUserByIdAsync(id);
            if (targetUser == null)
                return NotFound(new { error = "User not found or delete failed" });

            if (targetUser.IsAdmin)
            {
                var adminCount = await GetAdminCountAsync();
                if (adminCount <= 1)
                    return BadRequest(new { error = "Cannot delete the last admin user" });
            }

            var success = await _userRepository.DeleteUserAsync(id);
            if (!success) return NotFound(new { error = "User not found or delete failed" });
            return Ok(new { message = "User deleted" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete user {UserId}", id);
            return StatusCode(500, new { error = "Failed to delete user" });
        }
        finally
        {
            AdminMutationLock.Release();
        }
    }

    [HttpPost("{id}/reset-password-link")]
    public async Task<IActionResult> GenerateResetPasswordLink(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User id is required" });

        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                var listedUser = await FindUserByIdAsync(id);
                if (listedUser != null)
                {
                    return BadRequest(new
                    {
                        error = "Reset link is not available for this legacy user account. This user is visible in admin, but is not backed by the ASP.NET Identity store."
                    });
                }

                return NotFound(new { error = "User not found" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var link = $"{Request.Scheme}://{Request.Host}/auth/reset-password?userId={Uri.EscapeDataString(user.Id)}&token={Uri.EscapeDataString(encodedToken)}";

            return Ok(new
            {
                message = "Password reset link generated",
                resetLink = link,
                expiresHint = "Default ASP.NET Identity token lifetime applies"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate reset link for user {UserId}", id);
            return StatusCode(500, new { error = "Failed to generate reset password link" });
        }
    }

    [HttpPatch("{id}/admin")]
    public async Task<IActionResult> MarkAdmin(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User id is required" });

        await AdminMutationLock.WaitAsync();
        try
        {
            var success = await _userRepository.SetAdminStatusAsync(id, true);
            if (!success) return NotFound(new { error = "User not found or update failed" });
            return Ok(new { message = "User marked as admin" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to promote user {UserId} to admin", id);
            return StatusCode(500, new { error = "Failed to update admin status" });
        }
        finally
        {
            AdminMutationLock.Release();
        }
    }

    [HttpPatch("{id}/remove-admin")]
    public async Task<IActionResult> RemoveAdmin(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { error = "User id is required" });

        await AdminMutationLock.WaitAsync();
        try
        {
            // Prevent removing own admin role
            var currentUserId = NormalizeUserId(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
            var targetUserId = NormalizeUserId(id);

            if (currentUserId == targetUserId)
                return BadRequest(new { error = "Cannot remove your own admin role" });

            var targetUser = await FindUserByIdAsync(id);
            if (targetUser == null)
                return NotFound(new { error = "User not found or update failed" });

            if (!targetUser.IsAdmin)
                return BadRequest(new { error = "User is not an admin" });

            var adminCount = await GetAdminCountAsync();
            if (adminCount <= 1)
                return BadRequest(new { error = "Cannot remove admin role from the last admin user" });

            var success = await _userRepository.SetAdminStatusAsync(id, false);
            if (!success) return NotFound(new { error = "User not found or update failed" });
            return Ok(new { message = "Admin role removed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove admin role for user {UserId}", id);
            return StatusCode(500, new { error = "Failed to update admin status" });
        }
        finally
        {
            AdminMutationLock.Release();
        }
    }

    private async Task<ApplicationUser?> FindUserByIdAsync(string id)
    {
        var normalizedId = NormalizeUserId(id);

        // Prefer repository direct lookup first (Identity store path).
        var user = await _userRepository.GetByIdAsync(normalizedId);
        if (user != null)
        {
            return user;
        }

        // Fallback to list-based lookup (legacy Node.js documents path).
        var users = await _userRepository.GetAllAsync();
        return users.FirstOrDefault(u => NormalizeUserId(u.Id) == normalizedId);
    }

    private async Task<int> GetAdminCountAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Count(u => u.IsAdmin);
    }

    private static string NormalizeUserId(string? id)
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

    public record UpdateUserProfileRequest(
        [Required, StringLength(64)] string Username,
        [Required, EmailAddress, StringLength(254)] string Email,
        [Required, StringLength(60)] string FirstName,
        [Required, StringLength(60)] string LastName,
        [StringLength(20)] string? Phone);
}
