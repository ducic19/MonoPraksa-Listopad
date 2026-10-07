using WebApplication3.Model;

namespace WebApplication3.Service.Common;

public interface ISongService
{
    IEnumerable<Song> GetAll();
    Song? GetById(int id);
    IEnumerable<Song> Search(string? artist, string? title, string? album);
    Song? Update(int id, Song updates);
    Song? Create(Song newSong);
    bool Delete(int id);
}