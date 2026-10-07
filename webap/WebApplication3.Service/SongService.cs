using WebApplication3.Common;
using WebApplication3.Model;
using WebApplication3.Repository.Common;
using WebApplication3.Service.Common;

namespace WebApplication3.Service;

public class SongService : ISongService
{
    private readonly ISongRepository _repository;
    private readonly IIdGenerator _idGenerator;
    private readonly ILoggerNotifier _notifier;

    public SongService(ISongRepository repository, IIdGenerator idGenerator, ILoggerNotifier notifier)
    {
        _repository = repository;
        _idGenerator = idGenerator;
        _notifier = notifier;
    }

    public IEnumerable<Song> GetAll() => _repository.GetAll();

    public Song? GetById(int id) => _repository.GetById(id);

    public IEnumerable<Song> Search(string? artist, string? title, string? album) 
        => _repository.Search(artist, title, album);

    public Song? Update(int id, Song updates)
    {
        var updated = _repository.Update(id, updates);
        if (updated != null)
        {
            _notifier.Notify($"Ažurirana pjesma s ID-em {id}");
        }
        return updated;
    }

    public Song? Create(Song newSong)
    {
        if (string.IsNullOrWhiteSpace(newSong.Artist) || 
            string.IsNullOrWhiteSpace(newSong.Title) || 
            string.IsNullOrWhiteSpace(newSong.Album))
        {
            return null;
        }

        var currentIds = _repository.GetAll().Select(s => s.ID);
        newSong.ID = _idGenerator.GetNextId(currentIds);

        var created = _repository.Add(newSong);
        _notifier.Notify($"Kreirana nova pjesma s ID-em {created.ID}");
        return created;
    }

    public bool Delete(int id)
    {
        var success = _repository.Delete(id);
        if (success)
        {
            _notifier.Notify($"Obrisana pjesma s ID-em {id}");
        }
        return success;
    }
}