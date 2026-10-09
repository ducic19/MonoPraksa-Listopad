using gymap.Model;
using gymap.Repository.Common;
using Microsoft.EntityFrameworkCore;

namespace gymap.Repository
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _context;

        public MemberRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1. FILTRIRANJE U BAZI (koristi Name)
        public async Task<List<Member>> GetActiveMembersAsync(string searchName)
        {
            return await _context.Members
                .Where(m => m.Name.Contains(searchName)) // Točno polje Name
                .ToListAsync();
        }

        // 2. JOIN TABLICA (koristi Subs i MemId)
        public async Task<Member?> GetMemberWithDetailsAsync(Guid id)
        {
            return await _context.Members
                .Include(m => m.Subs) // Točna relacija Subs iz Member.cs
                .FirstOrDefaultAsync(m => m.MemId == id); // Točan primarni ključ MemId
        }

        // 3. PARALELNI TASKOVI
        public async Task<(List<Member> Members, List<Trainer> Trainers)> GetDashboardDataAsync()
        {
            var members = await _context.Members.ToListAsync();
            var trainers = await _context.Trainers.ToListAsync();

            return (members, trainers);
        }

        public async Task AddMemberAsync(Member member)
        {
            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMemberAsync(Member member)
        {
            _context.Members.Update(member);
            await _context.SaveChangesAsync();
        }
    }
}