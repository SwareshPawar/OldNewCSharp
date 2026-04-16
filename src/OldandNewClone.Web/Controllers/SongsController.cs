using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OldandNewClone.Application.Interfaces;
using OldandNewClone.Application.DTOs;

namespace OldandNewClone.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongsController : ControllerBase
{
    private readonly ISongService _songService;

    public SongsController(ISongService songService)
    {
        _songService = songService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var songs = await _songService.GetAllSongsAsync();
        return Ok(songs);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var song = await _songService.GetSongByIdAsync(id);
        if (song == null) return NotFound();
        return Ok(song);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateSongDto createSongDto)
    {
        var song = await _songService.CreateSongAsync(createSongDto);
        return CreatedAtAction(nameof(GetById), new { id = song.Id }, song);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSongDto updateSongDto)
    {
        if (id != updateSongDto.Id) return BadRequest();
        var result = await _songService.UpdateSongAsync(updateSongDto);
        if (!result) return NotFound();
        return Ok(new { message = "Song updated" });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _songService.DeleteSongAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Song deleted" });
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Search term required");
        var songs = await _songService.SearchSongsAsync(q);
        return Ok(songs);
    }

    [HttpGet("filter")]
    [AllowAnonymous]
    public async Task<IActionResult> Filter([FromQuery] string? category, [FromQuery] string? key, [FromQuery] List<string>? genres)
    {
        var songs = await _songService.FilterSongsAsync(category, key, genres);
        return Ok(songs);
    }
}
