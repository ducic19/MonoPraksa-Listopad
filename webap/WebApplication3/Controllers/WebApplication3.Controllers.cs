using Microsoft.AspNetCore.Mvc;
using WebApplication3.Model;
using WebApplication3.Service.Common;

namespace WebApplication3.Controllers;

[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly ISongService _songService;

    public SongsController(ISongService songService)
    {
        _songService = songService;
    }

    [HttpGet(Name = "GetSongs")]
    public IActionResult GetSongs()
    {
        return Ok(_songService.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetSong([FromRoute] int id)
    {
        var song = _songService.GetById(id);
        if (song == null) return NotFound();

        return Ok(song);
    }

    [HttpGet("search")]
    public IActionResult SearchSongs([FromQuery] string? artist, [FromQuery] string? title, [FromQuery] string? album)
    {
        var res = _songService.Search(artist, title, album);
        return Ok(res);
    }

    [HttpPut("{id}", Name = "UpdateSong")]
    public IActionResult UpdateSong([FromRoute] int id, [FromBody] Song updates)
    {
        var updatedSong = _songService.Update(id, updates);
        if (updatedSong == null) return NotFound();

        return Ok(updatedSong);
    }

    [HttpPost]
    public IActionResult CreateSong([FromBody] Song newsong)
    {
        var createdSong = _songService.Create(newsong);
        if (createdSong == null) return BadRequest("Nevaljani podaci.");

        return CreatedAtAction(nameof(GetSong), new { id = createdSong.ID }, createdSong);
    }

    [HttpDelete("{id}", Name = "DeleteSong")]
    public IActionResult DeleteSong([FromRoute] int id)
    {
        var success = _songService.Delete(id);
        if (!success) return NotFound();

        return NoContent();
    }
}