using Gymap.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Gymap.DAL.Members
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _context;

        public MemberRepository(AppDbContext context)
        {
            _context = context;
        }

        // Filtriranje ide u bazu, rezultat je ograničen na 100 zapisa.
        public async Task<List<Member>> SearchAsync(string searchName)
        {
            return await _context.Members
                .Where(m => m.Name.ToLower().Contains(searchName))
                .OrderBy(m => m.Name)
                .Take(100)
                .ToListAsync();
        }

        public async Task<Member?> GetByIdAsync(Guid id)
        {
            return await _context.Members
                .Include(m => m.Subs)
                .FirstOrDefaultAsync(m => m.MemId == id);
        }

        // E-mail se sprema normaliziran (trim + lowercase), ali postojeći zapisi
        // mogu imati velika slova pa usporedba ide preko lower().
        public Task<Member?> GetByEmailAsync(string email) =>
            _context.Members.FirstOrDefaultAsync(m => m.Email.ToLower() == email);

        public Task<bool> EmailExistsAsync(string email) =>
            _context.Members.AnyAsync(m => m.Email.ToLower() == email);

        public Task<bool> EmailExistsForOtherAsync(string email, Guid memId) =>
            _context.Members.AnyAsync(m => m.MemId != memId && m.Email.ToLower() == email);

        public async Task AddAsync(Member member)
        {
            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        // Vraća false kad jedinstveni indeks na e-mail odbije upis (utrka dviju registracija).
        public async Task<bool> TryAddAsync(Member member)
        {
            try
            {
                _context.Members.Add(member);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                _context.Entry(member).State = EntityState.Detached;
                return false;
            }
        }

        public async Task UpdateAsync(Member member)
        {
            // Samo član se označava kao izmijenjen, učitan Subs ostaje netaknut.
            _context.Entry(member).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var member = await _context.Members.FindAsync(id);

            if (member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
        }
    }
}
