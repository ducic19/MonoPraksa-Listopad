namespace Gymap.Common.DTO.Members;

public record MemberResponseDto(
    Guid MemId,
    string Name,
    string Email,
    string Role,
    Guid? SubsId);
