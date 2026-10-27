using Gymap.Common;
using Microsoft.EntityFrameworkCore;

namespace Gymap.DAL.Subscriptions
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subscription>> GetAllAsync()
        {
            return await _context.Subscriptions
                .OrderBy(s => s.Price)
                .ToListAsync();
        }

        public Task<Subscription?> GetByIdAsync(Guid id) =>
            _context.Subscriptions.FirstOrDefaultAsync(s => s.SubsId == id);

        public Task<bool> ExistsAsync(Guid id) =>
            _context.Subscriptions.AnyAsync(s => s.SubsId == id);

        // Pretplata se ne smije obrisati dok je vezana na članove (FK bi to odbio).
        public Task<bool> IsInUseAsync(Guid id) =>
            _context.Members.AnyAsync(m => m.SubsId == id);

        public async Task AddAsync(Subscription subscription)
        {
            await _context.Subscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Subscription subscription)
        {
            _context.Entry(subscription).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);

            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }
        }
    }
}
