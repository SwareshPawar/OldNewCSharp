using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Web.Services;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserCheckController : ControllerBase
{
    private readonly MongoDbUserChecker _userChecker;
    private readonly ILogger<UserCheckController> _logger;

    public UserCheckController(
        MongoDbUserChecker userChecker,
        ILogger<UserCheckController> logger)
    {
        _userChecker = userChecker;
        _logger = logger;
    }

    [HttpGet("check/{emailOrUsername}")]
    public async Task<IActionResult> CheckUser(string emailOrUsername)
    {
        var result = await _userChecker.CheckUserAsync(emailOrUsername);
        return Ok(result);
    }

    [HttpPost("test-login")]
    public async Task<IActionResult> TestLogin([FromBody] TestLoginRequest request)
    {
        var userCheck = await _userChecker.CheckUserAsync(request.EmailOrUsername);

        if (!userCheck.Found)
        {
            return Ok(new
            {
                success = false,
                message = "User not found",
                details = userCheck
            });
        }

        var passwordMatches = await _userChecker.TestPasswordAsync(request.EmailOrUsername, request.Password);

        return Ok(new
        {
            success = passwordMatches,
            message = passwordMatches ? "✅ Password matches!" : "❌ Password does not match",
            user = userCheck,
            passwordMatches
        });
    }
}

public class TestLoginRequest
{
    public string EmailOrUsername { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
