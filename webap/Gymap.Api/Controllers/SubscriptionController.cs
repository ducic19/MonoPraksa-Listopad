using Gymap.Common;
using Gymap.Common.DTO.Subscriptions;
using Gymap.Service;
using Gymap.Service.Subscriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subscriptionService.GetAllAsync();
            return Ok(result.Select(ToResponse));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var subscription = await _subscriptionService.GetByIdAsync(id);
            if (subscription == null) return NotFound("Pretplata nije pronađena.");

            return Ok(ToResponse(subscription));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubscriptionAddDto dto)
        {
            var subscription = await _subscriptionService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = subscription.SubsId }, ToResponse(subscription));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] SubscriptionEditDto dto)
        {
            return (await _subscriptionService.UpdateAsync(id, dto)) switch
            {
                SaveResult.NotFound => NotFound("Pretplata nije pronađena."),
                _ => NoContent()
            };
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return (await _subscriptionService.DeleteAsync(id)) switch
            {
                SaveResult.NotFound => NotFound("Pretplata nije pronađena."),
                SaveResult.InUse => Conflict("Pretplata je dodijeljena članovima i ne može se obrisati."),
                _ => NoContent()
            };
        }

        private static SubscriptionDto ToResponse(Subscription subscription) =>
            new(subscription.SubsId, subscription.Weekly, subscription.Duration, subscription.Price);
    }
}
