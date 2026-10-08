using WebApplication3.Model;
using WebApplication3.Repository.Common;
using WebApplication3.Service.Common;

namespace WebApplication3.Service
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<List<Member>> GetActiveMembersAsync(string searchName)
        {
            return await _memberRepository.GetActiveMembersAsync(searchName);
        }

        public async Task<Member?> GetMemberWithDetailsAsync(Guid id)
        {
            return await _memberRepository.GetMemberWithDetailsAsync(id);
        }

        public async Task<(List<Member> Members, List<Trainer> Trainers)> GetDashboardDataAsync()
        {
            return await _memberRepository.GetDashboardDataAsync();
        }
    }
}