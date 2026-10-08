using Microsoft.EntityFrameworkCore;
using WebApplication3.Model;
using WebApplication3.Repository.Common;

namespace WebApplication3.Repository
{
    public class SongRepository : ISongRepository
    {
        private readonly AppDbContext _context;

        public SongRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Member>> GetAllAsync()
        {
            return await _context.Members.ToListAsync();
        }

        public async Task<List<Trainer>> GetAllEmployeesAsync()
        {
            return await _context.Trainers.ToListAsync();
        }
    }
}