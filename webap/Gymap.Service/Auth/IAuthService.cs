using Gymap.Common.DTO.Auth;

namespace Gymap.Service.Auth;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(UserRegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(UserLoginDto dto);
}
