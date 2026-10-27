using Gymap.Common;

namespace Gymap.DAL.Members;

public interface IMemberRepository
{
    Task<List<Member>> SearchAsync(string searchName);
    Task<Member?> GetByIdAsync(Guid id);
    Task<Member?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> EmailExistsForOtherAsync(string email, Guid memId);
    Task AddAsync(Member member);
    Task<bool> TryAddAsync(Member member);
    Task UpdateAsync(Member member);
    Task DeleteAsync(Guid id);
}
