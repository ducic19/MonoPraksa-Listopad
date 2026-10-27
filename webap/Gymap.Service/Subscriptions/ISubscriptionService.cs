using Gymap.Common;
using Gymap.Common.DTO.Subscriptions;

namespace Gymap.Service.Subscriptions;

public interface ISubscriptionService
{
    Task<List<Subscription>> GetAllAsync();
    Task<Subscription?> GetByIdAsync(Guid id);
    Task<Subscription> CreateAsync(SubscriptionAddDto dto);
    Task<SaveResult> UpdateAsync(Guid id, SubscriptionEditDto dto);
    Task<SaveResult> DeleteAsync(Guid id);
}
