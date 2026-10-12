namespace gymap.DTO
{
    public record UserResponseDto(
        Guid Id,
        string Name,
        string Email,
        string Role,
        string Token
    );
}