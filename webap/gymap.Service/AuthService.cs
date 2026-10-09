using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using DTO;
using gymap.Model;
using gymap.Repository.Common;
using gymap.Service.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace gymap.DTO
{
    public class AuthService : IAuthService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IMemberRepository memberRepository, IConfiguration configuration)
        {
            _memberRepository = memberRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserRegisterDto dto)
        {
            // 1. Provjera postoji li već korisnik s tim e-mailom
            var existingMembers = await _memberRepository.GetActiveMembersAsync("");
            if (existingMembers.Any(m => m.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return null; // Korisnik već postoji
            }

            // 2. Hashiranje lozinke pomoću BCrypta (automatski generira i spaja Salt!)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 3. Stvaranje novog člana s ulogom "User"
            var member = new Member
            {
                MemId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = "User" // Defaultna uloga (klijent ne bira sam svoju ulogu)
            };

            await _memberRepository.AddMemberAsync(member);

            // 4. Generiranje i vraćanje JWT tokena
            var token = CreateJwtToken(member);

            return new AuthResponseDto
            {
                Token = token,
                Email = member.Email,
                Role = member.Role
            };
        }

        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto dto)
        {
            var members = await _memberRepository.GetActiveMembersAsync("");
            var member = members.FirstOrDefault(m => m.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));

            if (member == null) return null;

            // Usporedba unesene lozinke i hasha u bazi
            bool isValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, member.PasswordHash);
            if (!isValidPassword) return null;

            var token = CreateJwtToken(member);

            return new AuthResponseDto
            {
                Token = token,
                Email = member.Email,
                Role = member.Role
            };
        }

        private string CreateJwtToken(Member member)
        {
            // Ključ MORA imati minimalno 16 znakova (128 bita), stavljamo iznimno dugacki ključ
            var secretKey = _configuration["JwtSettings:Secret"];
    
            if (string.IsNullOrEmpty(secretKey) || Encoding.UTF8.GetBytes(secretKey).Length < 16)
            {
                secretKey = "OvoJeVrloSiguranITajniKljucKojiImaViseOd256BitaZaJWT!";
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, member.MemId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, member.Email ?? ""),
                new Claim(ClaimTypes.Role, member.Role ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"] ?? "gymap",
                audience: _configuration["JwtSettings:Audience"] ?? "gymap",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}