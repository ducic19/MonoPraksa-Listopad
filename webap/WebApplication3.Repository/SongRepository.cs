using WebApplication3.Model;
using WebApplication3.Repository.Common;

namespace WebApplication3.Repository;

public class SongRepository : ISongRepository
{
    private static readonly List<Song> _songs = new List<Song>
    {
        new Song { ID = 1, Title = "In the Dark", Artist = "Billy Squier", Album = "Don't say no" },
        new Song { ID = 2, Title = "Echoes", Artist = "Pink Floyd", Album = "Meddle" },
        new Song { ID = 3, Title = "Let it happen", Artist = "Tame Impala", Album = "Currents" },
    };

    public IEnumerable<Song> GetAll() => _songs;

    public Song? GetById(int id) => _songs.FirstOrDefault(s => s.ID == id);

    public IEnumerable<Song> Search(string? artist, string? title, string? album)
    {
        var res = _songs.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(artist))
            res = res.Where(s => s.Artist.Contains(artist, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(title))
            res = res.Where(s => s.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(album))
            res = res.Where(s => s.Album.Contains(album, StringComparison.OrdinalIgnoreCase));

        return res;
    }

    public Song? Update(int id, Song updates)
    {
        var existingS = GetById(id);
        if (existingS == null) return null;

        existingS.Title = updates.Title;
        existingS.Artist = updates.Artist;
        existingS.Album = updates.Album;

        return existingS;
    }

    public Song Add(Song newSong)
    {
        _songs.Add(newSong);
        return newSong;
    }

    public bool Delete(int id)
    {
        var existingS = GetById(id);
        if (existingS == null) return false;

        _songs.Remove(existingS);
        return true;
    }
}