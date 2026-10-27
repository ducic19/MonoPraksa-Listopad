using Gymap.Common;
using Microsoft.EntityFrameworkCore;

namespace Gymap.DAL.Trainers
{
    public class TrainerRepository : ITrainerRepository
    {
        private readonly AppDbContext _context;

        public TrainerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Trainer>> GetAllAsync()
        {
            return await _context.Trainers
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<Trainer>> SearchAsync(string searchName)
        {
            return await _context.Trainers
                .Where(t => t.Name.ToLower().Contains(searchName))
                .OrderBy(t => t.Name)
                .Take(100)
                .ToListAsync();
        }

        public Task<Trainer?> GetByIdAsync(Guid id) =>
            _context.Trainers.FirstOrDefaultAsync(t => t.TrainerId == id);

        // Trener se ne briše dok je dodijeljen na pretplate (trainer_subs).
        public Task<bool> IsInUseAsync(Guid id) =>
            _context.Trainers
                .Where(t => t.TrainerId == id)
                .SelectMany(t => t.Subs)
                .AnyAsync();

        public async Task AddAsync(Trainer trainer)
        {
            await _context.Trainers.AddAsync(trainer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Trainer trainer)
        {
            _context.Entry(trainer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
