using gymap.Model;

namespace gymap.Repository.Common
{
    public interface IMemberRepository
    {
        Task<List<Member>> GetActiveMembersAsync(string searchName);
        Task<Member?> GetMemberWithDetailsAsync(Guid id); // Promijenjeno u Guid!
        Task<(List<Member> Members, List<Trainer> Trainers)> GetDashboardDataAsync();
        Task AddMemberAsync(Member member);
        Task UpdateMemberAsync(Member member);
    }
}