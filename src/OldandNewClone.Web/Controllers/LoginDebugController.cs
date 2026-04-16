using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginDebugController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LoginDebugController> _logger;

    public LoginDebugController(
        IUserRepository userRepository,
        ILogger<LoginDebugController> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet("test-lookup/{usernameOrEmail}")]
    public async Task<IActionResult> TestLookup(string usernameOrEmail)
    {
        try
        {
            _logger.LogInformation("Testing user lookup for: {UsernameOrEmail}", usernameOrEmail);

            var loginInput = usernameOrEmail.ToLower();

            // Test standard lookup
            var standardUser = await _userRepository.GetByUsernameOrEmailAsync(loginInput);

            // Test hybrid lookup
            var hybridUser = await _userRepository.GetByUsernameOrEmailHybridAsync(loginInput);

            return Ok(new
            {
                input = usernameOrEmail,
                normalizedInput = loginInput,
                standardLookup = standardUser != null ? new
                {
                    found = true,
                    id = standardUser.Id,
                    userName = standardUser.UserName,
                    email = standardUser.Email,
                    hasPasswordHash = !string.IsNullOrEmpty(standardUser.PasswordHash),
                    passwordHashPrefix = standardUser.PasswordHash?.Substring(0, Math.Min(20, standardUser.PasswordHash.Length))
                } : null,
                hybridLookup = hybridUser != null ? new
                {
                    found = true,
                    id = hybridUser.Id,
                    userName = hybridUser.UserName,
                    email = hybridUser.Email,
                    firstName = hybridUser.FirstName,
                    lastName = hybridUser.LastName,
                    hasPasswordHash = !string.IsNullOrEmpty(hybridUser.PasswordHash),
                    passwordHashPrefix = hybridUser.PasswordHash?.Substring(0, Math.Min(20, hybridUser.PasswordHash.Length))
                } : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing lookup");
            return StatusCode(500, new
            {
                error = ex.Message,
                type = ex.GetType().Name,
                stackTrace = ex.StackTrace,
                innerError = ex.InnerException?.Message
            });
        }
    }
}
