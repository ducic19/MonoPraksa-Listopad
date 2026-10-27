using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gymap.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gymap.Service.Auth
{
    public class TokenService : ITokenService
    {
        private const int DefaultExpirationMinutes = 120;
        private const int MinimumKeyBytes = 32;

        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(Member user)
        {
            var secretKey = _config["JwtSettings:Key"]
                            ?? throw new InvalidOperationException("JwtSettings:Key nije definiran!");

            if (Encoding.UTF8.GetByteCount(secretKey) < MinimumKeyBytes)
            {
                throw new InvalidOperationException(
                    $"JwtSettings:Key mora imati najmanje {MinimumKeyBytes} bajta.");
            }

            var expirationMinutes = int.TryParse(_config["JwtSettings:ExpirationInMinutes"], out var minutes)
                ? minutes
                : DefaultExpirationMinutes;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.MemId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, NormalizeRole(user.Role))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Postojeći zapisi mogu imati "admin" umjesto "Admin", a provjera rola je case-sensitive.
        private static string NormalizeRole(string? role) =>
            string.Equals(role, Roles.Admin, StringComparison.OrdinalIgnoreCase)
                ? Roles.Admin
                : Roles.User;
    }
}
