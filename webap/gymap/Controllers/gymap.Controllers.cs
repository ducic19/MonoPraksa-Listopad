using Microsoft.AspNetCore.Authorization;
using gymap.Service.Common;
using Microsoft.AspNetCore.Mvc;
using gymap.Model;
using DTO;

namespace gymap.Controllers
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

        // Bilo koji prijavljeni korisnik može pretraživati članove
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var result = await _memberService.GetActiveMembersAsync(name ?? "");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var member = await _memberService.GetMemberWithDetailsAsync(id);
            if (member == null) return NotFound("Član nije pronađen.");
            return Ok(member);
        }

        // Samo Admin smije stvarati nove članove
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MemberAddDto dto)
        {
            var newMember = new Member
            {
                MemId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                SubsId = dto.SubsId
            };

            await _memberService.AddMemberAsync(newMember);
            return CreatedAtAction(nameof(GetDetails), new { id = newMember.MemId }, newMember);
        }
        
    }
}