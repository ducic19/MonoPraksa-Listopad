using Gymap.Common;

namespace Gymap.DAL.Subscriptions;

public interface ISubscriptionRepository
{
    Task<List<Subscription>> GetAllAsync();
    Task<Subscription?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> IsInUseAsync(Guid id);
    Task AddAsync(Subscription subscription);
    Task UpdateAsync(Subscription subscription);
    Task DeleteAsync(Guid id);
}
