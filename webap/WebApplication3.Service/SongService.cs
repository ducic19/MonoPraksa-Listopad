using WebApplication3.Model;
using WebApplication3.Repository.Common;
using WebApplication3.Service.Common;

namespace WebApplication3.Service
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;

        public SongService(ISongRepository songRepository)
        {
            _songRepository = songRepository;
        }

        public async Task<List<Member>> GetAllAsync()
        {
            return await _songRepository.GetAllAsync();
        }

        public async Task<List<Trainer>> GetAllEmployeesAsync()
        {
            return await _songRepository.GetAllEmployeesAsync();
        }
    }
}