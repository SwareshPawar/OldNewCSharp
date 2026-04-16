using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Application.DTOs;
using OldandNewClone.Application.Interfaces;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(registerDto);

        if (!result.Success)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(loginDto);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

        if (!result.Success)
        {
            return Unauthorized(new { message = result.ErrorMessage });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var result = await _authService.RevokeTokenAsync(userId);

        if (!result)
        {
            return BadRequest(new { message = "Failed to revoke token" });
        }

        return Ok(new { message = "Token revoked successfully" });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new
        {
            id = userId,
            email = email,
            role = role
        });
    }
}
