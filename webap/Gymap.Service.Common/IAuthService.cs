using System.Threading.Tasks;
using DTO;

namespace gymap.Service.Common
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(UserRegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(UserLoginDto dto);
    }
}