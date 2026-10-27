namespace Gymap.Common.DTO.Subscriptions;

public record SubscriptionDto(
    Guid SubsId,
    int Weekly,
    int Duration,
    decimal Price);
