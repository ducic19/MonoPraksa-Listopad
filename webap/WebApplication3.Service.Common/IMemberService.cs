using WebApplication3.Model;

namespace WebApplication3.Service.Common
{
    public interface IMemberService
    {
        Task<List<Member>> GetActiveMembersAsync(string searchName);
        Task<Member?> GetMemberWithDetailsAsync(Guid id); // Promijenjeno u Guid!
        Task<(List<Member> Members, List<Trainer> Trainers)> GetDashboardDataAsync();
    }
}