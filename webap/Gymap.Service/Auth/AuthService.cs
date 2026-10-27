using Gymap.Common;
using Gymap.Common.DTO.Auth;
using Gymap.DAL.Members;

namespace Gymap.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ITokenService _tokenService;

        public AuthService(IMemberRepository memberRepository, ITokenService tokenService)
        {
            _memberRepository = memberRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserRegisterDto dto)
        {
            var email = NormalizeEmail(dto.Email);
            if (await _memberRepository.EmailExistsAsync(email))
            {
                return null; // Korisnik s tim e-mailom već postoji
            }

            var member = new Member
            {
                MemId = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = Roles.User
            };

            // Jedinstveni indeks hvata i utrku između provjere i upisa.
            if (!await _memberRepository.TryAddAsync(member))
            {
                return null;
            }

            return ToResponse(member);
        }

        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto dto)
        {
            var member = await _memberRepository.GetByEmailAsync(NormalizeEmail(dto.Email));

            // Članove koje je kreirao admin nemaju lozinku pa se ne mogu prijaviti.
            if (member is null || string.IsNullOrWhiteSpace(member.PasswordHash))
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, member.PasswordHash))
            {
                return null;
            }

            return ToResponse(member);
        }

        private AuthResponseDto ToResponse(Member member) => new()
        {
            Token = _tokenService.CreateToken(member),
            Email = member.Email,
            Role = NormalizeRole(member.Role)
        };

        private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

        // Postojeći zapisi mogu imati "admin" umjesto "Admin".
        private static string NormalizeRole(string? role) =>
            string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase)
                ? Roles.Admin
                : Roles.User;
    }
}
