using Microsoft.AspNetCore.Mvc;
namespace webapSongs.Controllers;

[ApiController]
[Route("[controller]")]
public class webapSongs : ControllerBase
{
    private static List<Song> _songs = new List<Song>
    {
        new Song { ID = 1, Title = "In the Dark", Artist = "Billy Squier", Album = "Don't say no" },
        new Song { ID = 2, Title = "Echoes", Artist = "Pink Floyd", Album = "Meddle" },
        new Song { ID = 3, Title = "Let it happen", Artist = "Tame Impala", Album = "Currents" },
    };
//GET 
    [HttpGet(Name = "GetSongs")]
    public IActionResult GetSongs()
    {
        return Ok(_songs); //vraća listu svih pjesama
    }

    [HttpGet("{id}")]
    public IActionResult GetSong([FromRoute] int id)
    {
        var existingS = _songs.FirstOrDefault(s => s.ID == id);
        if (existingS == null)
            {
            return NotFound();
            }
        return Ok(existingS);
    }

    [HttpGet("search")]
    public IActionResult SearchSongs([FromQuery] string? artist, [FromQuery] string? title, [FromQuery] string? album)
    {
        var res = _songs.AsEnumerable();
        if (artist!=null)
        {
            res=res.Where(s => s.Artist.Contains(artist));
        }

        if (title!=null)
        {
            res = res.Where(s => s.Title.Contains(title));
        }

        if (album!=null)
        {
            res = res.Where(s => s.Album.Contains(album));
        }
        
        return Ok(res);
    }
    //PUT 
    [HttpPut("{id}", Name = "UpdateSong")]
    public IActionResult UpdateSong([FromRoute] int id, [FromBody] Song updates)
    {
        var existingS=_songs.FirstOrDefault(s => s.ID == id);
        if (existingS == null)
        {
            return NotFound();
        }
        existingS.Title=updates.Title;
        existingS.Artist=updates.Artist;
        existingS.Album=updates.Album;
        return Ok(existingS);
    }
    //POST
    [HttpPost]
    public IActionResult CreateSong([FromBody] Song newsong)
    {
        if (string.IsNullOrWhiteSpace(newsong.Artist) || string.IsNullOrWhiteSpace(newsong.Title) ||
            string.IsNullOrWhiteSpace(newsong.Album))
        {
            return BadRequest();
        }
        newsong.ID = _songs.Max(s=>s.ID) + 1; //count +1 mozda duplicira neke id
        _songs.Add(newsong);
        return Ok(newsong);
    }
    //DELETE
    [HttpDelete("{id}", Name = "DeleteSong")]
    public IActionResult DeleteSong([FromRoute] int id)
    {
        var existingS = _songs.FirstOrDefault(s => s.ID == id);
        if (existingS == null)
        {
            return NotFound();
        }
        _songs.Remove(existingS);
        return NoContent();
    }
}