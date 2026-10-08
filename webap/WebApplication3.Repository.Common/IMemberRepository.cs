using WebApplication3.Model;

namespace WebApplication3.Repository.Common
{
    public interface IMemberRepository
    {
        Task<List<Member>> GetActiveMembersAsync(string searchName);
        Task<Member?> GetMemberWithDetailsAsync(Guid id); // Promijenjeno u Guid!
        Task<(List<Member> Members, List<Trainer> Trainers)> GetDashboardDataAsync();
    }
}