using Microsoft.AspNetCore.Mvc;
using WebApplication3.Model;
using WebApplication3.Service.Common;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var result = await _memberService.GetActiveMembersAsync(name ?? "");
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var member = await _memberService.GetMemberWithDetailsAsync(id);
            if (member == null) return NotFound("Član nije pronađen.");
            return Ok(member);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var (members, trainers) = await _memberService.GetDashboardDataAsync();
            return Ok(new { Members = members, Trainers = trainers });
        }
        
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees()
        {
            var (_, trainers) = await _memberService.GetDashboardDataAsync();
            return Ok(trainers);
        }
    }
}