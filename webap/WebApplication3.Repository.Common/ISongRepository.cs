using WebApplication3.Model;

namespace WebApplication3.Repository.Common;

public interface ISongRepository
{
    IEnumerable<Song> GetAll();
    Song? GetById(int id);
    IEnumerable<Song> Search(string? artist, string? title, string? album);
    Song? Update(int id, Song updates);
    Song Add(Song newSong);
    bool Delete(int id);
}