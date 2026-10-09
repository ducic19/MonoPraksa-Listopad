using DTO;
using gymap.DTO;
using gymap.Service.Common;
using Microsoft.AspNetCore.Mvc;

namespace gymap.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result == null) return BadRequest("Korisnik s tim e-mailom već postoji.");
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null) return Unauthorized("Neispravan e-mail ili lozinka.");
            return Ok(result);
        }
    }
}