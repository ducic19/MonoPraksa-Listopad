using gymap.Model;
using gymap.Repository.Common;
using gymap.Service.Common;

namespace gymap.Service
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

        public async Task AddMemberAsync(Member member)
        {
            await _memberRepository.AddMemberAsync(member);
        }

        public async Task UpdateMemberAsync(Member member)
        {
            await _memberRepository.UpdateMemberAsync(member);
        }
    }
}