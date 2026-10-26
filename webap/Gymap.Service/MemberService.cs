using DTO;
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

        public async Task UpdateAsync(Guid id, MemberEditDto dto)
        {
            // 1. Dohvati postojećeg člana prema ID-u
            var member = await _memberRepository.GetMemberWithDetailsAsync(id);

            if (member != null)
            {
                // 2. Ažuriraj samo ona svojstva koja dolaze kroz DTO
                member.Name = dto.Name;
                member.Email = dto.Email;
                // Ovdje možeš dodati i ostala polja ako ih MemberEditDto sadrži (npr. telefon, adresa i sl.)

                // 3. Spremi promjene u bazu preko repozitorija
                await _memberRepository.UpdateMemberAsync(member);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            // 1. Dohvati člana prema ID-u (provjeri postoji li)
            var member = await _memberRepository.GetMemberWithDetailsAsync(id);
    
            if (member != null)
            {
                // 2. Izbriši člana iz repozitorija
                await _memberRepository.DeleteMemberAsync(id); 
            }
        }
    }
}