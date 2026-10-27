using Gymap.Common.DTO.Subscriptions;

namespace Gymap.Common.DTO.Members;

public record MemberDetailsDto(
    Guid MemId,
    string Name,
    string Email,
    string Role,
    SubscriptionDto? Subscription);
