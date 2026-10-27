using Gymap.Common;
using Gymap.Common.DTO.Members;

namespace Gymap.Service.Members;

public interface IMemberService
{
    Task<List<Member>> SearchAsync(string name);
    Task<Member?> GetByIdAsync(Guid id);
    Task<(SaveResult Result, Member? Member)> CreateAsync(MemberAddDto dto);
    Task<SaveResult> UpdateAsync(Guid id, MemberEditDto dto);
    Task<(SaveResult Result, Member? Member)> SetSubscriptionAsync(Guid memberId, Guid subsId);
    Task<SaveResult> DeleteAsync(Guid id);
}
