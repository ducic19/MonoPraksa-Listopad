using Gymap.Common;
using Gymap.Common.DTO.Members;
using Gymap.DAL.Members;
using Gymap.DAL.Subscriptions;

namespace Gymap.Service.Members
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public MemberService(
            IMemberRepository memberRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _memberRepository = memberRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<List<Member>> SearchAsync(string name)
        {
            return await _memberRepository.SearchAsync(name.Trim().ToLowerInvariant());
        }

        public async Task<Member?> GetByIdAsync(Guid id)
        {
            return await _memberRepository.GetByIdAsync(id);
        }

        public async Task<(SaveResult Result, Member? Member)> CreateAsync(MemberAddDto dto)
        {
            // Član ne smije završiti s pretplatom koja ne postoji.
            if (!await _subscriptionRepository.ExistsAsync(dto.SubsId))
            {
                return (SaveResult.InvalidReference, null);
            }

            var email = NormalizeEmail(dto.Email);
            if (await _memberRepository.EmailExistsAsync(email))
            {
                return (SaveResult.Duplicate, null);
            }

            var member = new Member
            {
                MemId = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Email = email,
                Role = Roles.User,
                SubsId = dto.SubsId
            };

            // Jedinstveni indeks hvata i utrku između provjere i upisa.
            if (!await _memberRepository.TryAddAsync(member))
            {
                return (SaveResult.Duplicate, null);
            }

            return (SaveResult.Success, member);
        }

        public async Task<SaveResult> UpdateAsync(Guid id, MemberEditDto dto)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member is null) return SaveResult.NotFound;

            if (!await _subscriptionRepository.ExistsAsync(dto.SubsId))
            {
                return SaveResult.InvalidReference;
            }

            var email = NormalizeEmail(dto.Email);
            if (await _memberRepository.EmailExistsForOtherAsync(email, id))
            {
                return SaveResult.Duplicate;
            }

            member.Name = dto.Name.Trim();
            member.Email = email;
            member.SubsId = dto.SubsId;

            await _memberRepository.UpdateAsync(member);
            return SaveResult.Success;
        }

        // Član sam bira ili mijenja svoju članarinu; admin i dalje koristi UpdateAsync.
        public async Task<(SaveResult Result, Member? Member)> SetSubscriptionAsync(Guid memberId, Guid subsId)
        {
            if (!await _subscriptionRepository.ExistsAsync(subsId))
            {
                return (SaveResult.InvalidReference, null);
            }

            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member is null) return (SaveResult.NotFound, null);

            if (member.SubsId != subsId)
            {
                member.SubsId = subsId;
                await _memberRepository.UpdateAsync(member);
            }

            // Učitavamo odabranu pretplatu da odgovor odmah sadrži novi plan.
            member.Subs = await _subscriptionRepository.GetByIdAsync(subsId);
            return (SaveResult.Success, member);
        }

        public async Task<SaveResult> DeleteAsync(Guid id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member is null) return SaveResult.NotFound;

            await _memberRepository.DeleteAsync(id);
            return SaveResult.Success;
        }

        private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
    }
}
