using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Application.DTOs;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserDataController : ControllerBase
{
    private readonly IUserDataService _userDataService;

    public UserDataController(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(string userId)
    {
        var userData = await _userDataService.GetUserDataAsync(userId);
        return Ok(userData);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> Update(string userId, [FromBody] UpdateUserDataDto updateDto)
    {
        var userData = await _userDataService.UpdateUserDataAsync(userId, updateDto);
        return Ok(userData);
    }

    [HttpPost("{userId}/favorites/{songId}")]
    public async Task<IActionResult> ToggleFavorite(string userId, int songId)
    {
        var result = await _userDataService.ToggleFavoriteAsync(userId, songId);
        return Ok(new { success = result });
    }

    [HttpPost("{userId}/new-setlist/{songId}")]
    public async Task<IActionResult> ToggleNewSetlist(string userId, int songId)
    {
        var result = await _userDataService.ToggleNewSetlistAsync(userId, songId);
        return Ok(new { success = result });
    }

    [HttpPost("{userId}/old-setlist/{songId}")]
    public async Task<IActionResult> ToggleOldSetlist(string userId, int songId)
    {
        var result = await _userDataService.ToggleOldSetlistAsync(userId, songId);
        return Ok(new { success = result });
    }
}
