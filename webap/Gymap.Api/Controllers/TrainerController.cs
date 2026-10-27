using System.ComponentModel.DataAnnotations;
using Gymap.Common;
using Gymap.Common.DTO.Trainers;
using Gymap.Service;
using Gymap.Service.Trainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gymap.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainerController : ControllerBase
    {
        private readonly ITrainerService _trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        // Bez ?name= vraća sve trenere, inače filtrira po imenu.
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery, StringLength(100)] string? name)
        {
            var result = await _trainerService.SearchAsync(name);
            return Ok(result.Select(ToResponse));
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var trainer = await _trainerService.GetByIdAsync(id);
            if (trainer == null) return NotFound("Trener nije pronađen.");

            return Ok(ToResponse(trainer));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TrainerAddDto dto)
        {
            var trainer = await _trainerService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = trainer.TrainerId }, ToResponse(trainer));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TrainerEditDto dto)
        {
            return (await _trainerService.UpdateAsync(id, dto)) switch
            {
                SaveResult.NotFound => NotFound("Trener nije pronađen."),
                _ => NoContent()
            };
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return (await _trainerService.DeleteAsync(id)) switch
            {
                SaveResult.NotFound => NotFound("Trener nije pronađen."),
                SaveResult.InUse => Conflict("Trener je dodijeljen pretplatama i ne može se obrisati."),
                _ => NoContent()
            };
        }

        private static TrainerResponseDto ToResponse(Trainer trainer) =>
            new(trainer.TrainerId, trainer.Name, trainer.Email);
    }
}
