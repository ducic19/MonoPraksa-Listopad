using gymap.Service.Common;
using Microsoft.AspNetCore.Mvc;
using gymap.Model;
using DTO; // Donosi MemberAddDto i MemberEditDto

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

        // POST: api/member -> Stvaranje člana preko DTO-a
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

        // PUT: api/member/07d725b6-3e16-4ccd-affa-5833c7d9be1a -> Ažuriranje člana preko DTO-a
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MemberEditDto dto)
        {
            var existingMember = await _memberService.GetMemberWithDetailsAsync(id);
            if (existingMember == null) return NotFound("Član nije pronađen.");

            existingMember.Name = dto.Name;
            existingMember.Email = dto.Email;
            existingMember.SubsId = dto.SubsId;

            await _memberService.UpdateMemberAsync(existingMember);
            return Ok(existingMember);
        }
    }
}