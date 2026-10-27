using Gymap.Common;
using Gymap.Common.DTO.Trainers;
using Gymap.DAL.Trainers;

namespace Gymap.Service.Trainers
{
    public class TrainerService : ITrainerService
    {
        private readonly ITrainerRepository _trainerRepository;

        public TrainerService(ITrainerRepository trainerRepository)
        {
            _trainerRepository = trainerRepository;
        }

        // Bez upitanog imena vraća sve trenere, inače filtrira po imenu.
        public async Task<List<Trainer>> SearchAsync(string? name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? await _trainerRepository.GetAllAsync()
                : await _trainerRepository.SearchAsync(name.Trim().ToLowerInvariant());
        }

        public async Task<Trainer?> GetByIdAsync(Guid id)
        {
            return await _trainerRepository.GetByIdAsync(id);
        }

        public async Task<Trainer> CreateAsync(TrainerAddDto dto)
        {
            var trainer = new Trainer
            {
                TrainerId = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant()
            };

            await _trainerRepository.AddAsync(trainer);
            return trainer;
        }

        public async Task<SaveResult> UpdateAsync(Guid id, TrainerEditDto dto)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id);
            if (trainer is null) return SaveResult.NotFound;

            trainer.Name = dto.Name.Trim();
            trainer.Email = dto.Email.Trim().ToLowerInvariant();

            await _trainerRepository.UpdateAsync(trainer);
            return SaveResult.Success;
        }

        public async Task<SaveResult> DeleteAsync(Guid id)
        {
            var trainer = await _trainerRepository.GetByIdAsync(id);
            if (trainer is null) return SaveResult.NotFound;
            if (await _trainerRepository.IsInUseAsync(id)) return SaveResult.InUse;

            await _trainerRepository.DeleteAsync(id);
            return SaveResult.Success;
        }
    }
}
