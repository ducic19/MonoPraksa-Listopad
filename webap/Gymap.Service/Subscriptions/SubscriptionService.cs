using Gymap.Common;
using Gymap.Common.DTO.Subscriptions;
using Gymap.DAL.Subscriptions;

namespace Gymap.Service.Subscriptions
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<List<Subscription>> GetAllAsync()
        {
            return await _subscriptionRepository.GetAllAsync();
        }

        public async Task<Subscription?> GetByIdAsync(Guid id)
        {
            return await _subscriptionRepository.GetByIdAsync(id);
        }

        public async Task<Subscription> CreateAsync(SubscriptionAddDto dto)
        {
            var subscription = new Subscription
            {
                SubsId = Guid.NewGuid(),
                Weekly = dto.Weekly,
                Duration = dto.Duration,
                Price = dto.Price
            };

            await _subscriptionRepository.AddAsync(subscription);
            return subscription;
        }

        public async Task<SaveResult> UpdateAsync(Guid id, SubscriptionEditDto dto)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(id);
            if (subscription is null) return SaveResult.NotFound;

            subscription.Weekly = dto.Weekly;
            subscription.Duration = dto.Duration;
            subscription.Price = dto.Price;

            await _subscriptionRepository.UpdateAsync(subscription);
            return SaveResult.Success;
        }

        public async Task<SaveResult> DeleteAsync(Guid id)
        {
            if (!await _subscriptionRepository.ExistsAsync(id)) return SaveResult.NotFound;
            if (await _subscriptionRepository.IsInUseAsync(id)) return SaveResult.InUse;

            await _subscriptionRepository.DeleteAsync(id);
            return SaveResult.Success;
        }
    }
}
