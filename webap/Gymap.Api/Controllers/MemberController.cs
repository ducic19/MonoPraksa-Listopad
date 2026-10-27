using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Gymap.Common;
using Gymap.Common.DTO.Members;
using Gymap.Common.DTO.Subscriptions;
using Gymap.Service;
using Gymap.Service.Members;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymap.Api.Controllers
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

        // Bilo koji prijavljeni korisnik (User ili Admin) može pretraživati članove
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery, Required, StringLength(100, MinimumLength = 1)] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Unesite ime za pretragu.");

            var result = await _memberService.SearchAsync(name);
            return Ok(result.Select(ToResponse));
        }

        [Authorize]
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(Guid id)
        {
            var member = await _memberService.GetByIdAsync(id);
            if (member == null) return NotFound("Član nije pronađen.");

            return Ok(ToDetails(member));
        }

        // Svaki prijavljeni član može sam odabrati ili promijeniti svoju članarinu.
        [Authorize]
        [HttpPut("me/subscription/{subsId}")]
        public async Task<IActionResult> SetMySubscription(Guid subsId)
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(claim, out var memberId))
            {
                return Unauthorized("Neispravan identitet korisnika.");
            }

            var (result, member) = await _memberService.SetSubscriptionAsync(memberId, subsId);

            return result switch
            {
                SaveResult.NotFound => NotFound("Član nije pronađen."),
                SaveResult.InvalidReference => BadRequest("Pretplata ne postoji."),
                _ => Ok(ToDetails(member!))
            };
        }

        // Samo Admin smije stvarati nove članove (korištenje konstante)
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MemberAddDto dto)
        {
            var (result, member) = await _memberService.CreateAsync(dto);

            return result switch
            {
                SaveResult.InvalidReference => BadRequest("Pretplata ne postoji."),
                SaveResult.Duplicate => Conflict("Član s tim e-mailom već postoji."),
                _ => CreatedAtAction(nameof(GetDetails), new { id = member!.MemId }, ToResponse(member))
            };
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] MemberEditDto dto)
        {
            return (await _memberService.UpdateAsync(id, dto)) switch
            {
                SaveResult.NotFound => NotFound("Član nije pronađen."),
                SaveResult.InvalidReference => BadRequest("Pretplata ne postoji."),
                SaveResult.Duplicate => Conflict("Član s tim e-mailom već postoji."),
                _ => NoContent()
            };
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return (await _memberService.DeleteAsync(id)) switch
            {
                SaveResult.NotFound => NotFound("Član nije pronađen."),
                _ => NoContent()
            };
        }

        private static MemberResponseDto ToResponse(Member member) =>
            new(member.MemId, member.Name, member.Email, member.Role, member.SubsId);

        private static MemberDetailsDto ToDetails(Member member)
        {
            var subscription = member.Subs is null
                ? null
                : new SubscriptionDto(member.Subs.SubsId, member.Subs.Weekly, member.Subs.Duration, member.Subs.Price);

            return new MemberDetailsDto(
                member.MemId,
                member.Name,
                member.Email,
                member.Role,
                subscription);
        }
    }
}
