using Microsoft.AspNetCore.Mvc;
using WebApplication3.Model;
using WebApplication3.Service.Common;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly ISongService _songService;

        public MemberController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var members = await _songService.GetAllAsync();
            return Ok(members);
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _songService.GetAllEmployeesAsync();
            return Ok(employees);
        }
    }
}