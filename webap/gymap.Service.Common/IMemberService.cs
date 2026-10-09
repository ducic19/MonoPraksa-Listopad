using gymap.Model;

namespace gymap.Service.Common
{
    public interface IMemberService
    {
        Task<List<Member>> GetActiveMembersAsync(string searchName);
        Task<Member?> GetMemberWithDetailsAsync(Guid id); // Promijenjeno u Guid!
        Task<(List<Member> Members, List<Trainer> Trainers)> GetDashboardDataAsync();
        
        Task AddMemberAsync(Member member);
        Task UpdateMemberAsync(Member member);
    }
}