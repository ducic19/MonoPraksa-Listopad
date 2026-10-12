using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using gymap.Model;
using gymap.Service.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace gymap.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(Member user)
        {
            var secretKey = _config["JwtSettings:Secret"] 
                            ?? throw new InvalidOperationException("JwtSettings:Secret nije definiran!");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MemId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // Inicijalizacija role u tokenu
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}