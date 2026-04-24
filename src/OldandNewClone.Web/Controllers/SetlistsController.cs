using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Application.DTOs;
using OldandNewClone.Application.Interfaces;
using System.Security.Claims;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SetlistsController : ControllerBase
{
    private readonly ISetlistService _setlistService;

    public SetlistsController(ISetlistService setlistService)
    {
        _setlistService = setlistService;
    }

    [HttpGet("global")]
    public async Task<IActionResult> GetGlobal() => Ok(await _setlistService.GetGlobalSetlistsAsync());

    [HttpGet("mine")]
    public async Task<IActionResult> GetMine() => Ok(await _setlistService.GetMySetlistsAsync(GetUserId()));

    [HttpGet("smart")]
    public async Task<IActionResult> GetSmart() => Ok(await _setlistService.GetSmartSetlistsAsync(GetUserId()));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var setlist = await _setlistService.GetByIdAsync(id);
        return setlist is null ? NotFound() : Ok(setlist);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSetlistDto dto)
    {
        var created = await _setlistService.CreateAsync(GetUserId(), User.IsInRole("Admin"), dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateSetlistDto dto)
    {
        var updated = await _setlistService.UpdateAsync(GetUserId(), User.IsInRole("Admin"), id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id}/songs/{songId}")]
    public async Task<IActionResult> AddSong(string id, int songId)
    {
        try
        {
            var updated = await _setlistService.AddSongAsync(GetUserId(), User.IsInRole("Admin"), id, songId);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}/songs/{songId}")]
    public async Task<IActionResult> RemoveSong(string id, int songId)
    {
        try
        {
            var updated = await _setlistService.RemoveSongAsync(GetUserId(), User.IsInRole("Admin"), id, songId);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _setlistService.DeleteAsync(GetUserId(), User.IsInRole("Admin"), id);
        return deleted ? Ok(new { success = true }) : NotFound();
    }

    [HttpPost("{id}/sync")]
    public async Task<IActionResult> SyncSmartSetlist(string id)
    {
        var updated = await _setlistService.SyncSmartSetlistAsync(GetUserId(), User.IsInRole("Admin"), id);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPost("smart-scan")]
    public async Task<IActionResult> SmartScan([FromBody] Dictionary<string, string>? conditions)
    {
        var ids = await _setlistService.PreviewSmartSongIdsAsync(conditions);
        return Ok(ids);
    }

    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? string.Empty;
}
